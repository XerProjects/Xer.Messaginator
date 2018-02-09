using System;

namespace ConsoleApp.Entities
{
    public class SampleMessage
    {
        public int Id { get; }

        public SampleMessage()
        {
            Id = GetHashCode();
        }
    }
}