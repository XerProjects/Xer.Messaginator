using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xer.Messaginator.MessageSources.Http
{
    public interface ISerializer<TMessage, TResult>
    {
        TResult Serialize(TMessage message);
        Task<TResult> SerializeAsync(TMessage message, CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IStringSerializer<TMessage> : ISerializer<TMessage, string>
    {
    }

    public interface IByteSerializer<TMessage> : ISerializer<TMessage, byte[]>
    {
    }

    public interface IStreamSerializer<TMessage> : ISerializer<TMessage, Stream>
    {
    }
}