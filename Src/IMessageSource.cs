using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Messaginator
{
    public delegate Task MessageReceivedDelegate<TMessage>(MessageContainer<TMessage> message) where TMessage : class;

    /// <summary>
    /// Represents a source of messages.
    /// </summary>
    public interface IMessageSource<TMessage> where TMessage : class
    {   
        /// <summary>
        /// Received messages are published through this event.
        /// </summary>
        /// <remarks>
        /// It is recommended that subscribers of event handler should not 
        /// let any exceptions propagate because it may not be observed.
        /// </remarks>
        event MessageReceivedDelegate<TMessage> MessageReceived;

        /// <summary>
        /// Exceptions that occurred while receiving messages are published through this event.
        /// </summary>
        event EventHandler<Exception> OnError;

        /// <summary>
        /// Start receiving messages from the source.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task which that can be awaited for completion.</returns>
        Task StartReceivingAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Stop receiving messages from the source.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task which can be awaited for completion.</returns>
        Task StopReceivingAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Receive a message in-process and schedule for processing.
        /// </summary>
        /// <param name="message">Message to receive.</param>
        /// <returns>Task which can be awaited for completion.</returns>
        Task ReceiveAsync(MessageContainer<TMessage> message);
    }    
}