using System.Threading.Tasks;
using Xer.Delegator;
using Xer.Delegator.Registration;

namespace Xer.Messaginator
{
    /// <summary>
    /// Represents a builder for <see cref="MessageProcessorHost"/>.
    /// </summary>
    public class MessageProcessorHostBuilder
    {
        private MultiMessageHandlerRegistration _registration = new MultiMessageHandlerRegistration();

        /// <summary>
        /// Add message processor to the host.
        /// </summary>
        /// <param name="messageProcessor">Message processor.</param>
        /// <returns>Message processor builder.</returns>
        public MessageProcessorHostBuilder AddMessageProcessor<TMessage>(MessageProcessor<TMessage> messageProcessor) where TMessage : class
        {
            if (messageProcessor == null)
            {
                throw new System.ArgumentNullException(nameof(messageProcessor));
            }

            // Hook to internal messages.
            _registration.Register<StartMessageProcessingMessage>((message, ct) => messageProcessor.StartAsync(message.MessageProcessorHost, ct));
            _registration.Register<StopMessageProcessingMessage>((message, ct) => messageProcessor.StopAsync(ct));
            _registration.Register<ForwardToMessageProcessorMessage<TMessage>>((message, ct) => 
            {
                // Receive if message if it is meant for this message processor.
                if (messageProcessor.Name == message.MessageProcessorName)
                {
                    return messageProcessor.ReceiveMessageAsync(message.MessageToForward, ct);
                }

                return TaskUtility.CompletedTask;
            });

            return this;
        }

        /// <summary>
        /// Build the <see cref="MessageProcessorHost"/> instance.
        /// </summary>
        /// <returns>Instance of <see cref="MessageProcessorHost"/>.</returns>
        public MessageProcessorHost Build()
        {
            IMessageHandlerResolver resolver = _registration.BuildMessageHandlerResolver();
            IMessageDelegator messageDelegator = new MessageDelegator(resolver);
            return new MessageProcessorHost(messageDelegator);
        }
    }
}