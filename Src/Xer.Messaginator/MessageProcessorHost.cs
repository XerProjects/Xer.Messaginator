using System;
using System.Threading;
using System.Threading.Tasks;
using Xer.Delegator;
using Xer.Delegator.Registration;

namespace Xer.Messaginator
{
    /// <summary>
    /// Represents an object that hosts message processors.
    /// </summary>
    public class MessageProcessorHost
    {
        private IMessageDelegator _messageDelegator;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageDelegator">Message delegator.</param>
        internal MessageProcessorHost(IMessageDelegator messageDelegator)
        {
            _messageDelegator = messageDelegator;
        }

        /// <summary>
        /// Forward message to a message processor.
        /// </summary>
        /// <param name="messageProcessorName">Name of message processor.</param>
        /// <param name="messageToForward">Message to forward.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Asynchronous task that completes when the message has been forwarded.</returns>
        public Task ForwardMessageAsync<TMessage>(string messageProcessorName, 
                                                  MessageContainer<TMessage> messageToForward, 
                                                  CancellationToken cancellationToken = default(CancellationToken)) where TMessage : class
        {
            if (messageProcessorName == null)
            {
                throw new ArgumentNullException(nameof(messageProcessorName));
            }

            if (messageToForward == null)
            {
                throw new ArgumentNullException(nameof(messageToForward));
            }

            return _messageDelegator.SendAsync(new ForwardToMessageProcessorMessage<TMessage>(messageProcessorName, messageToForward), 
                                               cancellationToken);
        }
        
        /// <summary>
        /// Start message processing.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Asynchronous task that completes when all message processors have been started.</returns>
        public Task StartAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _messageDelegator.SendAsync(new StartMessageProcessingMessage(this), cancellationToken);
        }

        /// <summary>
        /// Stop message processing.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Asynchronous task that completes when all message processors have been stopped.</returns>
        public Task StopAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _messageDelegator.SendAsync(new StopMessageProcessingMessage(this), cancellationToken);
        }
    }
}