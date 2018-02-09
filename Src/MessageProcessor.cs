using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Messaginator
{
    public abstract class MessageProcessor<TMessage> where TMessage : class
    {
        /// <summary>
        /// Internally cached message source derived from MessageSource.
        /// </summary>
        private IMessageSource<TMessage> _internalMessageSource;

        /// <summary>
        /// Source where message handler will subscribe to receive messages.
        /// </summary>
        protected abstract IMessageSource<TMessage> MessageSource { get; }
        
        /// <summary>
        /// Exceptions that occurred during processing are published through this event.
        /// </summary>
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// Start message handler.
        /// </summary>
        /// <remarks>This method returns an already completed task and should not block.</remarks>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completed task.</returns>
        public virtual Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Get an instance provided by child class and store in a private field. 
            _internalMessageSource = MessageSource ?? throw new InvalidOperationException("Message handler has no message source.");

            // Subscribe to messages.
            _internalMessageSource.MessageReceived += (receivedMessage) =>
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
            _internalMessageSource.OnError += (s, ex) => publishException(ex);

            OnStart();

            _internalMessageSource.StartReceivingAsync(cancellationToken);

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

            return _internalMessageSource.StopReceivingAsync(cancellationToken);
        }

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
    }
}