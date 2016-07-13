using System;
using System.Threading;

namespace SFA.DAS.Messaging
{
    public interface IEventingMessageReceiver<T> where T : new()
    {
        event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        void Start(CancellationToken cancellationToken);
    }
}