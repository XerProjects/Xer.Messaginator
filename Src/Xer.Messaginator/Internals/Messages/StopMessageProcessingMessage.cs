namespace Xer.Messaginator
{
    internal class StopMessageProcessingMessage
    {
        /// <summary>
        /// Message processor host.
        /// </summary>
        public MessageProcessorHost MessageProcessorHost { get; }

        /// <summary>
        /// Constuctor.
        /// </summary>
        /// <param name="host">Message processor host.</param>
        public StopMessageProcessingMessage(MessageProcessorHost host)
        {
            MessageProcessorHost = host ?? throw new System.ArgumentNullException(nameof(host));
        }
    }
}