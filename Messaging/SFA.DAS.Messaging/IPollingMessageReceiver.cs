using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public interface IPollingMessageReceiver
    {
        Task<Message<T>> ReceiveAsAsync<T>() where T : new();
        Task<IEnumerable<Message<T>>> ReceiveBatchAsAsync<T>(int batchSize) where T : new();
    }
}