using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Messaginator.MessageSources
{
    /// <summary>
    /// Represents a source of message.
    /// </summary>
    public abstract class MessageSource<TMessage> : IMessageSource<TMessage> where TMessage : class
    {
        /// <summary>
        /// Received messages are published through this event.
        /// </summary>
        /// <remarks>
        /// It is recommended that subscribers of event handler should not 
        /// let any exceptions propagate because it may not be observed.
        /// </remarks>
        public event MessageReceivedDelegate<TMessage> OnMessageReceived;

        /// <summary>
        /// Exceptions that occurred while receiving messages are published through this event.
        /// </summary>
        public event EventHandler<Exception> OnError;

        /// <summary>
        /// Receive a message and schedule for processing.
        /// </summary>
        /// <param name="message">Message to receive.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task which can be awaited for completion.</returns>
        public abstract Task ReceiveAsync(MessageContainer<TMessage> message, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Start receiving messages from the source.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Asynchronous task that can be awaited for completion.</returns>
        public abstract Task StartReceivingAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Stop receiving messages from the source.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Asynchronous task that can be awaited for completion.</returns>
        public abstract Task StopReceivingAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Publish received message.
        /// </summary>
        /// <param name="receivedMessage">Received message.</param>
        protected void PublishMessage(MessageContainer<TMessage> receivedMessage)
        {
            if (receivedMessage != null && !receivedMessage.IsEmpty)
            {
                // This is not awaited.
                OnMessageReceived?.Invoke(receivedMessage);
            }
        }

        /// <summary>
        /// Publish exception.
        /// </summary>
        /// <param name="exception">Exception.</param>
        protected void PublishException(Exception exception)
        {
            if (exception != null)
            {
                OnError?.Invoke(this, exception);
            }
        }
    }
}