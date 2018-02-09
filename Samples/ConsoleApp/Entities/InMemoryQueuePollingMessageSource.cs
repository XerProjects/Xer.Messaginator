using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xer.Messaginator;
using Xer.Messaginator.MessageSources;

namespace ConsoleApp.Entities
{
    public class InMemoryQueuePollingMessageSource : PollingMessageSource<SampleMessage>
    {
        private readonly Queue<SampleMessage> _queue;

        protected override TimeSpan PollingInterval { get; }

        public InMemoryQueuePollingMessageSource(Queue<SampleMessage> queue, TimeSpan pollingInterval)
        {
            _queue = queue;
            PollingInterval = pollingInterval;
        }

        protected override Task<MessageContainer<SampleMessage>> TryGetMessageAsync(CancellationToken cancellationToken)
        {
            if(_queue.TryDequeue(out SampleMessage message))
            {
                System.Console.WriteLine($"Message {message.Id}: Received by {GetType().Name}.");
                return Task.FromResult(new MessageContainer<SampleMessage>(message));
            }

            return Task.FromResult(MessageContainer<SampleMessage>.Empty);
        }
    }
}