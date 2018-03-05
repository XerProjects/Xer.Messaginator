using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xer.Messaginator;

namespace ConsoleApp.HttpMessageSource.Entities
{
    public class SampleMessageProcessor : MessageProcessor<SampleMessage>
    {
        public override string Name => "SampleMessageProcessor";

        protected override Func<IMessageSource<SampleMessage>> MessageSourceFactory { get; }

        public SampleMessageProcessor(IMessageSource<SampleMessage> messageSource)
        {
            MessageSourceFactory = () => messageSource;
        }

        protected override Task ProcessMessageAsync(MessageContainer<SampleMessage> receivedMessage, CancellationToken cancellationToken)
        {
            // Implicit conversion.
            SampleMessage message = receivedMessage;

            System.Console.WriteLine($"Message {message.Id}: Processed by {GetType().Name}.");

            return Task.CompletedTask;
        }
    }
}