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

        /// <summary>
        /// Create an implementation of <see cref="Xer.Messaginator.IMessageForwarder"/>
        /// that can forward messages among message processors in this host.
        /// </summary>
        /// <returns>Implementation of <see cref="Xer.Messaginator.IMessageForwarder"/>.</returns>
        public IMessageForwarder CreateMessageForwarder()
        {
            return new MessageProcessorHostForwarder(this);
        }

        /// <summary>
        /// Forward message to a message processor.
        /// </summary>
        /// <param name="recipientMessageProcessorName">Name of message processor to forward to.</param>
        /// <param name="messageToForward">Message to forward.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Asynchronous task that completes when the message has been forwarded.</returns>
        private Task ForwardToMessageProcessorAsync<TMessage>(string recipientMessageProcessorName, 
                                                              MessageContainer<TMessage> messageToForward, 
                                                              CancellationToken cancellationToken = default(CancellationToken)) where TMessage : class
        {
            if (string.IsNullOrEmpty(recipientMessageProcessorName))
            {
                throw new ArgumentException("No recipient message processor name is provided.", nameof(recipientMessageProcessorName));
            }

            if (messageToForward == null)
            {
                throw new ArgumentNullException(nameof(messageToForward));
            }

            return _messageDelegator.SendAsync(new ForwardToMessageProcessorMessage<TMessage>(recipientMessageProcessorName, messageToForward), 
                                               cancellationToken);
        }

        /// <summary>
        /// Represents an object that can forward messages among message processors in a message processor host.
        /// </summary>
        private class MessageProcessorHostForwarder : IMessageForwarder
        {
            private readonly MessageProcessorHost _messageProcessorHost;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="messageProcessorHost">Message processor host.</param>
            public MessageProcessorHostForwarder(MessageProcessorHost messageProcessorHost)
            {
                _messageProcessorHost = messageProcessorHost;
            }

            /// <summary>
            /// Forward message to a message processor.
            /// </summary>
            /// <typeparam name="TMessage">Type of message to forward.</typeparam>
            /// <param name="recipientMessageProcessorName">Name of message processor to forward to.</param>
            /// <param name="messageToForward">Message to forward.</param>
            /// <param name="cancellationToken">Cancellation token.</param>
            /// <returns>Task which can be awaited for completion.</returns>
            public Task ForwardMessageAsync<TMessage>(string recipientMessageProcessorName, MessageContainer<TMessage> messageToForward, CancellationToken cancellationToken = default(CancellationToken)) where TMessage : class
            {
                return _messageProcessorHost.ForwardToMessageProcessorAsync(recipientMessageProcessorName, messageToForward, cancellationToken);
            }
        }
    }
}