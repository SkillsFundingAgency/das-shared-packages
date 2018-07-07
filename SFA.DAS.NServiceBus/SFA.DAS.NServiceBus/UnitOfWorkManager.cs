using System;
using System.Linq;
using NServiceBus;

namespace SFA.DAS.NServiceBus
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly IDb _db;
        private readonly IMessageSession _messageSession;
        private readonly IUnitOfWorkContext _unitOfWorkContext;
        private readonly IOutbox _outbox;
        private IOutboxTransaction _transaction;

        public UnitOfWorkManager(IDb db, IMessageSession messageSession, IUnitOfWorkContext unitOfWorkContext, IOutbox outbox)
        {
            _db = db;
            _messageSession = messageSession;
            _unitOfWorkContext = unitOfWorkContext;
            _outbox = outbox;
        }

        public void Begin()
        {
            _transaction = _outbox.BeginTransactionAsync().GetAwaiter().GetResult();
        }

        public void End(Exception ex = null)
        {
            using (_transaction)
            {
                if (ex == null)
                {
                    _db.SaveChangesAsync().GetAwaiter().GetResult();

                    var events = _unitOfWorkContext.GetEvents().ToList();
                    var outboxMessage = events.Any() ? new OutboxMessage(events) : null;

                    if (outboxMessage != null)
                    {
                        _outbox.AddAsync(outboxMessage).GetAwaiter().GetResult();
                    }

                    _transaction.Commit();

                    if (outboxMessage != null)
                    {
                        var options = new SendOptions();

                        options.SetMessageId(outboxMessage.Id.ToString());

                        _messageSession.Send(new ProcessOutboxMessageCommand(), options).GetAwaiter().GetResult();
                    }
                }
            }
        }
    }
}