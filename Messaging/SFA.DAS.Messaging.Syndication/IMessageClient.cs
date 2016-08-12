using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Syndication
{
    public interface IMessageClient
    {
        Task<ClientMessage<T>> GetNextUnseenMessage<T>();
        Task<IEnumerable<ClientMessage<T>>> GetBatchOfUnseenMessages<T>(int batchSize);
    }
}
