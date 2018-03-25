using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xer.Messaginator;
using Xer.Messaginator.MessageSources;
using Xer.Messaginator.MessageSources.Queue;

namespace ConsoleApp.Entities
{
    public class InMemoryQueuePollingMessageSource : QueuePollingMessageSource<SampleMessage>
    {
        protected override TimeSpan PollingInterval { get; }

        public InMemoryQueuePollingMessageSource(Queue<SampleMessage> queue, TimeSpan pollingInterval)
            : base(new InMemoryQueueAdapter(queue))
        {
            PollingInterval = pollingInterval;
        }

        protected override async Task<MessageContainer<SampleMessage>> GetNextMessageAsync(CancellationToken cancellationToken)
        {
            MessageContainer<SampleMessage> message = await base.GetNextMessageAsync(cancellationToken);

            System.Console.WriteLine($"Message {message.Message.Id}: Received by {GetType().Name}.");

            return message;
        }
    }
}