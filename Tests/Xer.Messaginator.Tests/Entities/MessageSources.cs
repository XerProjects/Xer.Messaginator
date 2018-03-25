using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xer.Messaginator.MessageSources;
using Xer.Messaginator.MessageSources.Queue;

namespace Xer.Messaginator.Tests.Entities
{
    public class InMemoryQueuePollingMessageSource : QueuePollingMessageSource<TestMessage>
    {
        private readonly Queue<TestMessage> _queue;

        protected override TimeSpan PollingInterval { get; }

        public InMemoryQueuePollingMessageSource(Queue<TestMessage> queue, TimeSpan pollingInterval)
            : base(new InMemoryQueueAdapter(queue))
        {
            _queue = queue;
            PollingInterval = pollingInterval;
        }

        protected override Task<MessageContainer<TestMessage>> GetNextMessageAsync(CancellationToken cancellationToken)
        {
            if(_queue.TryDequeue(out TestMessage message))
            {
                System.Console.WriteLine($"Message {message.MessageId}: Received by {GetType().Name}.");
                return Task.FromResult(new MessageContainer<TestMessage>(message));
            }

            return Task.FromResult(MessageContainer<TestMessage>.Empty);
        }
    }

    public class InMemoryQueueAdapter : IQueueAdapter<TestMessage>
    {
        private readonly Queue<TestMessage> _queue;

        public InMemoryQueueAdapter(Queue<TestMessage> queue)
        {
            _queue = queue;
        }

        public Task<MessageContainer<TestMessage>> DequeueAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            TestMessage item = _queue.Dequeue();
            return Task.FromResult(new MessageContainer<TestMessage>(item));
        }
    }
}