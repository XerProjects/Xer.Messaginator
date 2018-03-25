namespace Xer.Messaginator
{
    internal class ForwardToMessageProcessorMessage<TMessage> where TMessage : class
    {
        /// <summary>
        /// Name of message processor to forward to.
        /// </summary>
        public string MessageProcessorName { get; }

        /// <summary>
        /// Message to forward.
        /// </summary>
        public MessageContainer<TMessage> MessageToForward { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messageProcessorNameToForwardTo">Name of message processor to forward to.</param>
        /// <param name="messageToForward">Message to forward.</param>
        public ForwardToMessageProcessorMessage(string messageProcessorNameToForwardTo, MessageContainer<TMessage> messageToForward) 
        {
            MessageProcessorName = messageProcessorNameToForwardTo ?? throw new System.ArgumentNullException(nameof(messageProcessorNameToForwardTo));
            MessageToForward = messageToForward ?? throw new System.ArgumentNullException(nameof(messageToForward));
        }
    }
}