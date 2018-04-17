namespace Xer.Messaginator
{
    /// <summary>
    /// Represents an entity that supports forwarding of messages.
    /// </summary>
    public interface ISupportMessageForwarding
    {
        /// <summary>
        /// Set message forwarder.
        /// </summary>
        /// <remarks>This method will be called if implementor is added to a <see cref="Xer.Messaginator.MessageProcessorHost"/>.</remarks>
        /// <param name="messageForwarder">Message forwarder.</param>
        void SetMessageForwarder(IMessageForwarder messageForwarder);
    }
}