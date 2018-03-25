namespace Xer.Messaginator
{
    internal class StartMessageProcessingMessage
    {
        /// <summary>
        /// Message processor host start message processors with.
        /// </summary>
        public MessageProcessorHost MessageProcessorHost { get; }

        /// <summary>
        /// Constuctor.
        /// </summary>
        /// <param name="host">Message processor host start message processors with.</param>
        public StartMessageProcessingMessage(MessageProcessorHost host)
        {
            MessageProcessorHost = host ?? throw new System.ArgumentNullException(nameof(host));
        }
    }
}