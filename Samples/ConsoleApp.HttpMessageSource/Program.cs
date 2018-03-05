using System;
using System.Net.Http;
using System.Threading.Tasks;
using ConsoleApp.HttpMessageSource.Entities;
using Newtonsoft.Json;
using Xer.Messaginator;
using Xer.Messaginator.MessageSources.Http;

namespace ConsoleApp.HttpMessageSource
{
    class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        static async Task MainAsync(string[] args)
        {
            
            // This message source will open a port to receive HTTP requests.
            IMessageSource<SampleMessage> messageSource = new HttpMessageSource<SampleMessage>("http://localhost:6007");

            // This message processor will process messages received and published by the message source.
            MessageProcessor<SampleMessage> messageProcessor = new SampleMessageProcessor(messageSource);

            Console.WriteLine("Press any key to start message processing.");
            Console.ReadLine();

            Console.WriteLine("Starting...");
            // Will not block.
            await messageProcessor.StartAsync();

            while(true)
            {
                Console.WriteLine("Enter number of messages to send:");
                string input = Console.ReadLine();

                if(string.Equals(input, "stop", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Stopping...");
                    // Wait til last message is finished.
                    await messageProcessor.StopAsync();
                    break;
                }

                if(int.TryParse(input, out int num))
                {
                    for(int i = 0; i < num; i++)
                    {
                        var message = new SampleMessage();
                        Console.WriteLine($"Message {message.Id}: Sent for processing.");

                        await Task.Delay(TimeSpan.FromMilliseconds(500));

                        // Send message to the SampleMessageHttpMessageSource.
                        await _httpClient.PostAsync("http://localhost:6007", new StringContent(JsonConvert.SerializeObject(message)));
                    }
                }
            }
        }
    }
}
