using System.Threading;
using System.Threading.Tasks;

namespace Xer.Messaginator
{
    public interface IMessageForwarder
    {
        /// <summary>
        /// Forward message to another message processor.
        /// </summary>
        /// <typeparam name="TMessage">Type of message to forward.</typeparam>
        /// <param name="recipientMessageProcessorName">Name of message processor to forward to.</param>
        /// <param name="messageToForward">Message to forward.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task which can be awaited for completion.</returns>
        Task ForwardMessageAsync<TMessage>(string recipientMessageProcessorName, 
                                           MessageContainer<TMessage> messageToForward, 
                                           CancellationToken cancellationToken = default(CancellationToken))
                                           where TMessage : class;
    }
}