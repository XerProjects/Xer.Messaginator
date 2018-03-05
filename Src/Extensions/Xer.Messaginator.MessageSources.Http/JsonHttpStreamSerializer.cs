using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xer.Messaginator.MessageSources.Http
{
    public class JsonHttpStreamSerializer<TMessage> : IStreamSerializer<TMessage>
    {
        public Stream Serialize(TMessage message)
        {
            Stream stream = new MemoryStream();

            using (StreamWriter writer = new StreamWriter(stream))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, message);
            }

            return stream;
        }

        public Task<Stream> SerializeAsync(TMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(Serialize(message));
        }
    }
}