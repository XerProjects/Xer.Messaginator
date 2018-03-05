using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Messaginator.MessageSources.Http
{
    public interface IDeserializer<TMessage, TResult>
    {
        TResult Deserialize(TMessage message);
        Task<TResult> DeserializeAsync(TMessage message, CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IStringDeserializer<TMessage> : IDeserializer<string, TMessage>
    {
    }

    public interface IByteDeserializer<TMessage> : IDeserializer<byte[], TMessage>
    {
    }

    public interface IStreamDeserializer<TMessage> : IDeserializer<Stream, TMessage>
    {
    }
}