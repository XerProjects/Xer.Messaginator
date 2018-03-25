using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Messaginator.Tests.Entities
{
    public class TestMessage
    {
        public Guid MessageId { get; } = Guid.NewGuid();
    }
}