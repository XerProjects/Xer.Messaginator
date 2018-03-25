using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xer.Messaginator.Tests.Entities;
using Xunit;

namespace Xer.Messaginator.Tests
{
    public class MessageProcessorHostTests
    {
        public class StartAsyncMethod
        {
            [Fact]
            public async Task ShouldStartAllMessageProcessorsAndForwardTheMessage()
            {
                // Message Flow:
                // TestMessageProcessor1 --> TestMessageProcessor2 --> TestMessageProcessor3

                // Message source for message processor 1
                var inMemoryQueue1 = new Queue<TestMessage>();
                var queueMessageSource1 = new InMemoryQueuePollingMessageSource(inMemoryQueue1, TimeSpan.FromSeconds(1));

                // Message source for message processor 2
                var inMemoryQueue2 = new Queue<TestMessage>();
                var queueMessageSource2 = new InMemoryQueuePollingMessageSource(inMemoryQueue2, TimeSpan.FromSeconds(1));

                // Message source for message processor 3
                var inMemoryQueue3 = new Queue<TestMessage>();
                var queueMessageSource3 = new InMemoryQueuePollingMessageSource(inMemoryQueue3, TimeSpan.FromSeconds(1));

                var messageProcessor1 = new TestMessageProcessor1(queueMessageSource1);
                var messageProcessor2 = new TestMessageProcessor2(queueMessageSource2);
                var messageProcessor3 = new TestMessageProcessor3(queueMessageSource3);

                MessageProcessorHost host = new MessageProcessorHostBuilder()
                    .AddMessageProcessor<TestMessage>(messageProcessor1)
                    .AddMessageProcessor<TestMessage>(messageProcessor2)
                    .AddMessageProcessor<TestMessage>(messageProcessor3)
                    .Build();

                await host.StartAsync();

                // Put message to message processor 1's queue.
                var testMessage = new TestMessage();
                inMemoryQueue1.Enqueue(testMessage);

                // Wait for message to be passed to message processor 3
                await Task.Delay(TimeSpan.FromSeconds(5));

                Assert.True(messageProcessor3.IsHoldingMessage(testMessage.MessageId));
            }
        }
    }
}
