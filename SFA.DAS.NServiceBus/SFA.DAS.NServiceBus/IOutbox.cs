using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus
{
    public interface IOutbox
    {
        Task<IOutboxTransaction> BeginTransactionAsync();
        Task AddAsync(OutboxMessage outboxMessage);
        Task<OutboxMessage> GetById(Guid id);
        Task<IEnumerable<Guid>> GetIdsToProcess();
    }
}