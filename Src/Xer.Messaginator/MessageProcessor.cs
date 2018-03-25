using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Messaginator
{
    public abstract class MessageProcessor<TMessage> where TMessage : class
    {
        #region Declarations

        private MessageProcessorHost _host;
        private CancellationToken _cancellationToken;

        #endregion Declarations

        #region Properties
        
        /// <summary>
        /// Message processor name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Source where message handler will subscribe to receive messages.
        /// </summary>
        protected IMessageSource<TMessage> MessageSource { get; }

        #endregion Properties

        #region Events

        /// <summary>
        /// Exceptions that occurred during processing are published through this event.
        /// </summary>
        public event EventHandler<Exception> OnError;

        #endregion Events

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageSource">Message processor's source of messages.</param>
        public MessageProcessor(IMessageSource<TMessage> messageSource)
        {
            MessageSource = messageSource ?? throw new ArgumentNullException(nameof(messageSource));
            
            // Subscribe to messages.
            MessageSource.OnMessageReceived += (receivedMessage) => OnMessageReceived(receivedMessage);

            // Subscribe to errors.
            MessageSource.OnError += (s, ex) => PublishException(ex);
        }

        #region Methods
        
        /// <summary>
        /// Start message handler.
        /// </summary>
        /// <remarks>This method returns an already completed task and should not block.</remarks>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completed task.</returns>
        public virtual Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            _cancellationToken = cancellationToken;

            try
            {
                OnStart();
            }
            catch(Exception ex)
            {
                return TaskUtility.FromException(ex);
            }

            MessageSource.StartReceivingAsync(cancellationToken);

            return TaskUtility.CompletedTask;
        }
             
        /// <summary>
        /// Start message handler with message processor host.
        /// </summary>
        /// <remarks>This method returns an already completed task and should not block.</remarks>
        /// <param name="host">Message processor host that will host this message processor.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Completed task.</returns>
        public virtual Task StartAsync(MessageProcessorHost host, CancellationToken cancellationToken = default(CancellationToken))
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));

            _cancellationToken = cancellationToken;

            try
            {
                OnStart();
            }
            catch(Exception ex)
            {
                return TaskUtility.FromException(ex);
            }

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
            try
            {
                OnStop();
            }
            catch(Exception ex)
            {
                return TaskUtility.FromException(ex);
            }

            return MessageSource.StopReceivingAsync(cancellationToken);
        }

        /// <summary>
        /// Receive a message in-process and schedule for processing.
        /// </summary>
        /// <param name="message">Message to receive.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task which can be awaited for completion.</returns>
        public Task ReceiveMessageAsync(MessageContainer<TMessage> message, CancellationToken cancellationToken = default(CancellationToken))
        {
            return MessageSource.ReceiveAsync(message, cancellationToken);
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
        /// Forward message to another message processor inside the message processor host, if available.
        /// </summary>
        /// <param name="messageProcessorName">Name of next message processor.</param>
        /// <param name="messageToForward">Message to forward.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task which can be awaited for completion.</returns>
        protected Task ForwardToMessageProcessor(string messageProcessorName, TMessage messageToForward, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _host?.ForwardMessageAsync<TMessage>(messageProcessorName, new MessageContainer<TMessage>(messageToForward));
        }

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
        /// Handle message that is received through <see cref="IMessageSource{TMessage}"/>.
        /// </summary>
        /// <param name="receivedMessage">Received message.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Asynchronous task that can be awaited for completion.</returns>
        private async Task OnMessageReceived(MessageContainer<TMessage> receivedMessage)
        {
            // Do not process if null or empty.
            if (receivedMessage != null && !receivedMessage.IsEmpty)
            {
                try
                {
                    // Process message.
                    await ProcessMessageAsync(receivedMessage, _cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    PublishException(ex);
                }
            }
        }

        /// <summary>
        /// Publish exception.
        /// </summary>
        /// <param name="ex">Exception to publish.</param>
        private void PublishException(Exception ex)
        {
            if (OnError != null)
            {
                OnError(this, ex);
            }
        }

        #endregion Functions
    }
}