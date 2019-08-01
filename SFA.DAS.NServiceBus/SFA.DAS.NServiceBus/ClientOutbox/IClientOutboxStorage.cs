using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Persistence;

namespace SFA.DAS.NServiceBus.ClientOutbox
{
    public interface IClientOutboxStorage
    {
        Task<IClientOutboxTransaction> BeginTransactionAsync();
        Task<ClientOutboxMessage> GetAsync(Guid messageId, SynchronizedStorageSession synchronizedStorageSession);
        Task<ClientOutboxMessage> GetAsync(Guid messageId);
        Task<IEnumerable<IClientOutboxMessageAwaitingDispatch>> GetAwaitingDispatchAsync();
        Task SetAsDispatchedAsync(Guid messageId, SynchronizedStorageSession synchronizedStorageSession);
        Task StoreAsync(ClientOutboxMessage clientOutboxMessage, IClientOutboxTransaction clientOutboxTransaction);
    }
}