using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Messaginator.Tests.Entities
{
    public class TestMessageProcessor1 : MessageProcessor<TestMessage>
    {
        private List<TestMessage> _messages = new List<TestMessage>();

        public override string Name => GetType().Name;

        public TestMessageProcessor1(IMessageSource<TestMessage> messageSource) 
            : base(messageSource)
        {
        }

        protected override Task ProcessMessageAsync(MessageContainer<TestMessage> receivedMessage, CancellationToken cancellationToken)
        {
            // Pass to TestMessageProcessor2.
            return ForwardToMessageProcessor("TestMessageProcessor2", receivedMessage, cancellationToken);
        }

        public bool IsHoldingMessage(Guid messageId)
        {
            return _messages.Any(m => m.MessageId == messageId);
        }
    }

    public class TestMessageProcessor2 : MessageProcessor<TestMessage>
    {
        private List<TestMessage> _messages = new List<TestMessage>();

        public override string Name => GetType().Name;

        public TestMessageProcessor2(IMessageSource<TestMessage> messageSource) 
            : base(messageSource)
        {
        }

        protected override Task ProcessMessageAsync(MessageContainer<TestMessage> receivedMessage, CancellationToken cancellationToken)
        {
            // Pass to TestMessageProcessor3.
            return ForwardToMessageProcessor("TestMessageProcessor3", receivedMessage, cancellationToken);
        }

        public bool IsHoldingMessage(Guid messageId)
        {
            return _messages.Any(m => m.MessageId == messageId);
        }
    }

    public class TestMessageProcessor3 : MessageProcessor<TestMessage>
    {
        private List<TestMessage> _messages = new List<TestMessage>();

        public override string Name => GetType().Name;

        public TestMessageProcessor3(IMessageSource<TestMessage> messageSource) 
            : base(messageSource)
        {
        }

        protected override Task ProcessMessageAsync(MessageContainer<TestMessage> receivedMessage, CancellationToken cancellationToken)
        {
            // Complete the processing. Do not pass anymore to other message processor.
            _messages.Add(receivedMessage);
            return Task.CompletedTask;
        }

        public bool IsHoldingMessage(Guid messageId)
        {
            return _messages.Any(m => m.MessageId == messageId);
        }
    }
}