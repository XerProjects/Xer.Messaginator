using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xer.Messaginator.MessageSources.Http
{
    public class JsonHttpStreamDeserializer<TMessage> : IStreamDeserializer<TMessage>
    {
        public TMessage Deserialize(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<TMessage>(jsonReader);
            }
        }

        public Task<TMessage> DeserializeAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(Deserialize(stream));
        }
    }
}