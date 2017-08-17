using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public interface IPollingMessageReceiver
    {
        Task<IMessage<T>> ReceiveAsAsync<T>() where T : new();
        Task<IEnumerable<IMessage<T>>> ReceiveBatchAsAsync<T>(int batchSize) where T : new();
    }
}