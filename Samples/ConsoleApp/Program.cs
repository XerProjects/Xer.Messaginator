using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleApp.Entities;
using Xer.Messaginator;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        static async Task MainAsync(string[] args)
        {
            // This queue will be the source of message for InMemoryQueuePollingMessageSource.
            Queue<SampleMessage> queue = new Queue<SampleMessage>();
            
            // This message source will check the queue for any newly enqueued message every 1 second.
            IMessageSource<SampleMessage> messageSource = new InMemoryQueuePollingMessageSource(queue, pollingInterval: TimeSpan.FromSeconds(1));

            // This message processor will process messages received and published by the message source.
            MessageProcessor<SampleMessage> messageProcessor = new SampleMessageProcessor(messageSource);

            Console.WriteLine("Press any key to start message processing.");
            Console.ReadLine();

            Console.WriteLine("Starting...");
            // Will not block.
            await messageProcessor.StartAsync();

            while(true)
            {
                Console.WriteLine("Enter number of messages to queue:");
                string input = Console.ReadLine();

                if(string.Equals(input, "stop", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Stopping...");
                    // Wait till last message is finished.
                    await messageProcessor.StopAsync();
                    break;
                }

                if(int.TryParse(input, out int num))
                {
                    for(int i = 0; i < num; i++)
                    {
                        var message = new SampleMessage();
                        Console.WriteLine($"Message {message.Id}: Queued for processing.");

                        // Newly queued message will be detected by the InMemoryQueuePollingMessageSource
                        // and will publish the new message to be processed by SampleMessageProcessor.
                        queue.Enqueue(message);
                    }
                }
            }
        }
    }
}
