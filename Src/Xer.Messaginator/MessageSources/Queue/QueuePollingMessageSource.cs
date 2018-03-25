using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Messaginator.MessageSources.Queue
{
    /// <summary>
    /// Represents a message source that polls a queue for messages.
    /// </summary>
    public abstract class QueuePollingMessageSource<TMessage> : PollingMessageSource<TMessage> where TMessage : class
    {
        /// <summary>
        /// Queue adapter.
        /// </summary>
        protected IQueueAdapter<TMessage> QueueAdapter { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="queueAdapter">Queue adapter.</param>
        public QueuePollingMessageSource(IQueueAdapter<TMessage> queueAdapter)
        {
            QueueAdapter = queueAdapter ?? throw new ArgumentNullException(nameof(queueAdapter));
        }

        /// <summary>
        /// Get a message from the queue adapter.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Message container that contains the actual message.</returns>
        protected override Task<MessageContainer<TMessage>> GetNextMessageAsync(CancellationToken cancellationToken)
        {
            return QueueAdapter.DequeueAsync(cancellationToken);
        }
    }
}