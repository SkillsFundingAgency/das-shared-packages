using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Persistence;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Models;

namespace SFA.DAS.NServiceBus.Features.ClientOutbox.Data
{
    public interface IClientOutboxStorageV2
    {
        Task<IClientOutboxTransaction> BeginTransactionAsync();
        Task<ClientOutboxMessageV2> GetAsync(Guid messageId, SynchronizedStorageSession synchronizedStorageSession);
        Task<IEnumerable<IClientOutboxMessageAwaitingDispatch>> GetAwaitingDispatchAsync();
        Task SetAsDispatchedAsync(Guid messageId);
        Task SetAsDispatchedAsync(Guid messageId, SynchronizedStorageSession synchronizedStorageSession);
        Task StoreAsync(ClientOutboxMessageV2 clientOutboxMessage, IClientOutboxTransaction clientOutboxTransaction);
        Task RemoveEntriesOlderThanAsync(DateTime oldest, CancellationToken cancellationToken);
    }
}