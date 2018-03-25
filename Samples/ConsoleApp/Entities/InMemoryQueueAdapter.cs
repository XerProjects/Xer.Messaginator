using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xer.Messaginator;
using Xer.Messaginator.MessageSources.Queue;

namespace ConsoleApp.Entities
{
    public class InMemoryQueueAdapter : IQueueAdapter<SampleMessage>
    {
        private readonly Queue<SampleMessage> _queue;
        public InMemoryQueueAdapter(Queue<SampleMessage> queue)
        {
            _queue = queue ?? throw new System.ArgumentNullException(nameof(queue));
        }
        
        public Task<MessageContainer<SampleMessage>> DequeueAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            SampleMessage item = _queue.Dequeue();
            return Task.FromResult(new MessageContainer<SampleMessage>(item));
        }
    }
}