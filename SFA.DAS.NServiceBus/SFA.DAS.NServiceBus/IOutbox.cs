using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus
{
    public interface IOutbox
    {
        Task<IOutboxTransaction> BeginTransactionAsync();
        Task<OutboxMessage> GetAsync(Guid messageId);
        Task<IEnumerable<IOutboxMessageAwaitingDispatch>> GetAwaitingDispatchAsync();
        Task SetAsDispatchedAsync(Guid messageId);
        Task StoreAsync(OutboxMessage outboxMessage, IOutboxTransaction outboxTransaction);
    }
}