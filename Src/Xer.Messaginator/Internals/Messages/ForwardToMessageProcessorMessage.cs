namespace Xer.Messaginator
{
    internal class ForwardToMessageProcessorMessage<TMessage> where TMessage : class
    {
        /// <summary>
        /// Name of message processor to forward to.
        /// </summary>
        public string RecipientMessageProcessorName { get; }

        /// <summary>
        /// Message to forward.
        /// </summary>
        public MessageContainer<TMessage> MessageToForward { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="recipientMessageProcessorName">Name of message processor to forward to.</param>
        /// <param name="messageToForward">Message to forward.</param>
        public ForwardToMessageProcessorMessage(string recipientMessageProcessorName, MessageContainer<TMessage> messageToForward) 
        {
            RecipientMessageProcessorName = recipientMessageProcessorName ?? throw new System.ArgumentNullException(nameof(recipientMessageProcessorName));
            MessageToForward = messageToForward ?? throw new System.ArgumentNullException(nameof(messageToForward));
        }
    }
}