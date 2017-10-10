using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public interface IEventingMessageReceiver<T> where T : new()
    {
        event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        void Start(CancellationToken cancellationToken);
        Task RunAsync(CancellationToken cancellationToken);
    }
}