using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Messaginator
{
    public abstract class MessageProcessor<TMessage> where TMessage : class
    {
        #region Declarations
        
        /// <summary>
        /// Internally cached message source derived from MessageSourceFactory.
        /// </summary>
        private IMessageSource<TMessage> _internalMessageSource;

        #endregion Declarations

        #region Properties
        
        /// <summary>
        /// Message processor name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Source where message handler will subscribe to receive messages.
        /// </summary>
        protected IMessageSource<TMessage> MessageSource
        {
            get
            {
                if(_internalMessageSource == null)
                {
                    // Get an instance provided by child class and store in a private field. 
                    _internalMessageSource = MessageSourceFactory?.Invoke() ?? throw new InvalidOperationException("Message handler has no message source.");
                }

                return _internalMessageSource;
            }
        }

        /// <summary>
        /// Factory delegate that returns an instance of <see cref="Xer.Messaginator.IMessageSource{TMessage}"/> when invoked.
        /// </summary>
        protected abstract Func<IMessageSource<TMessage>> MessageSourceFactory { get; }

        #endregion Properties
        
        #region Events
        
        /// <summary>
        /// Exceptions that occurred during processing are published through this event.
        /// </summary>
        public event EventHandler<Exception> OnError;

        #endregion Events

        #region Methods
        
        /// <summary>
        /// Start message handler.
        /// </summary>
        /// <remarks>This method returns an already completed task and should not block.</remarks>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completed task.</returns>
        public virtual Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Subscribe to messages.
            MessageSource.MessageReceived += (receivedMessage) =>
            {
                // Do not process if null or empty.
                if (receivedMessage != null && !receivedMessage.IsEmpty)
                {
                    // Process message. This is not awaited.
                    ProcessMessageAsync(receivedMessage, cancellationToken)
                        // Publish exception and return true to let framework know that exception was handled.
                        .ContinueWith(t => t.Exception.Handle(ex =>
                        {
                            publishException(ex); return true;
                        }), TaskContinuationOptions.OnlyOnFaulted);
                }

                return TaskUtility.CompletedTask;
            };

            // Subscribe to errors.
            MessageSource.OnError += (s, ex) => publishException(ex);

            OnStart();

            MessageSource.StartReceivingAsync(cancellationToken);

            return TaskUtility.CompletedTask;
        }

        /// <summary>
        /// Stop message handler.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task which can be awaited until the last received message has finished processing.</returns>
        public virtual Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            OnStop();

            return MessageSource.StopReceivingAsync(cancellationToken);
        }

        #endregion Methods

        #region Abstract Methods

        /// <summary>
        /// Process message asynchronously.
        /// </summary>
        /// <remarks>
        /// This method is not awaited by the message handler.
        /// It is recommended to not let any exceptions exit this method because
        /// any exceptions that exits this method will not be handled.
        /// </remarks>
        /// <param name="receivedMessage">Message received.</param>
        /// <param name="cancellationToken">Cancellation token. This is cancelled when hosted command handler is stopped.</param>
        /// <returns>Task which can be awaited for completion.</returns>
        protected abstract Task ProcessMessageAsync(MessageContainer<TMessage> receivedMessage, CancellationToken cancellationToken);

        #endregion Abstract Methods

        #region Protected Methods

        /// <summary>
        /// Hook that is executed before message handler is started.
        /// </summary>
        protected virtual void OnStart()
        {
        }

        /// <summary>
        /// Hook that is executed before message handler is stopped.
        /// </summary>
        protected virtual void OnStop()
        {
        }

        #endregion Protected Methods

        #region Functions

        /// <summary>
        /// Publish exception.
        /// </summary>
        /// <param name="ex">Exception to publish.</param>
        private void publishException(Exception ex)
        {
            if (OnError != null)
            {
                OnError(this, ex);
            }
        }

        #endregion Functions
    }
}