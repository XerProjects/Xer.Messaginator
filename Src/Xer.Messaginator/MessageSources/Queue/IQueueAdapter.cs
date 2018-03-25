using System.Threading;
using System.Threading.Tasks;

namespace Xer.Messaginator.MessageSources.Queue
{
    /// <summary>
    /// Represents an adapter to a queue.
    /// </summary>
    public interface IQueueAdapter<TMessage> where TMessage : class
    {
        /// <summary>
        /// Dequeue message.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Message container that contains the actual message.</returns>
        Task<MessageContainer<TMessage>> DequeueAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}