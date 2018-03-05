using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Xer.Messaginator.MessageSources.Http
{
    public class HttpMessageSource<TMessage> : IMessageSource<TMessage> where TMessage : class
    {
        private CancellationToken _receiveCancellationToken;
        private IWebHost _host;
        private readonly string _url;

        public virtual IStreamDeserializer<TMessage> Deserializer { get; } = new JsonHttpStreamDeserializer<TMessage>();

        public event MessageReceivedDelegate<TMessage> MessageReceived;
        public event EventHandler<Exception> OnError;

        public HttpMessageSource(string url)
        {
            _url = url;
        }

        public Task ReceiveAsync(MessageContainer<TMessage> message)
        {
            PublishMessage(message);
            return Task.CompletedTask;
        }

        public Task StartReceivingAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            _receiveCancellationToken = cancellationToken;
            
            _host = ConfigureWebHost(new WebHostBuilder());
 
            return _host.StartAsync(cancellationToken);
        }

        public Task StopReceivingAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _host.StopAsync(cancellationToken);
        }

        public void Dispose()
        {
            _host.Dispose();
        }     

        protected virtual IWebHost ConfigureWebHost(IWebHostBuilder webHostBuilder)
        {
            return new WebHostBuilder()
                .UseKestrel()
                .ConfigureLogging(logging => logging.AddConsole())
                .ConfigureServices(services => services.AddRouting())
                .Configure(app => app.UseRouter(ConfigureRouting))
                .UseUrls(_url)
                .Build();
        }

        protected virtual void ConfigureRouting(IRouteBuilder router)
        {
            // Wire in our HTTP endpoints
            // Map root directory with POST.
            router.MapPost(string.Empty, HandleHttpRequestAsync);
        }

        protected virtual async Task HandleHttpRequestAsync(HttpContext context)
        {
            TMessage receivedMessage = await Deserializer.DeserializeAsync(context.Request.Body).ConfigureAwait(false);
            PublishMessage(receivedMessage);
        }

        private void PublishMessage(TMessage receivedMessage)
        {
            if (receivedMessage != null)
            {
                if (MessageReceived != null)
                {
                    // Not awaited.
                    Task t = MessageReceived(receivedMessage);
                }
            }
        }
    }
}
