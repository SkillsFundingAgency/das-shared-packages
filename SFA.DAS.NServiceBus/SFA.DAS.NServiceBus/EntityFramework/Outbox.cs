using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.NServiceBus.MsSqlServer;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public class Outbox<T> : IOutbox where T : DbContext, IOutboxDbContext
    {
        private readonly IUnitOfWorkContext _unitOfWorkContext;
        private readonly DbConnection _connection;
        private readonly Lazy<T> _db;

        public Outbox(IUnitOfWorkContext unitOfWorkContext, DbConnection connection, Lazy<T> db)
        {
            _unitOfWorkContext = unitOfWorkContext;
            _connection = connection;
            _db = db;
        }

        public async Task<IOutboxTransaction> BeginTransactionAsync()
        {
            await _connection.TryOpenAsync().ConfigureAwait(false);

            var transaction = _connection.BeginTransaction();
            var outboxTransaction = new OutboxTransaction(transaction);

            _unitOfWorkContext.Set(_connection);
            _unitOfWorkContext.Set(transaction);

            return outboxTransaction;
        }

        public Task AddAsync(OutboxMessage outboxMessage)
        {
            _db.Value.OutboxMessages.Add(outboxMessage);

            return _db.Value.SaveChangesAsync();
        }

        public Task<OutboxMessage> GetById(Guid id)
        {
            return _db.Value.OutboxMessages.SingleAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Guid>> GetIdsToProcess()
        {
            var sent = DateTime.UtcNow.AddMinutes(-10);

            var ids = await _db.Value.OutboxMessages
                .Where(m => m.Sent <= sent && m.Published == null)
                .OrderBy(m => m.Sent)
                .Select(m => m.Id)
                .ToListAsync()
                .ConfigureAwait(false);

            return ids;
        }
    }
}