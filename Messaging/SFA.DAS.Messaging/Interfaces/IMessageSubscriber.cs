using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public interface IMessageSubscriber<T> : IDisposable where T : new()
    {
        Task<IMessage<T>> ReceiveAsAsync();
        Task<IEnumerable<IMessage<T>>> ReceiveBatchAsAsync(int batchSize);
    }
}