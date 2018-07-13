using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Settings;

namespace SFA.DAS.NServiceBus
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly IDb _db;
        private readonly IMessageSession _messageSession;
        private readonly IUnitOfWorkContext _unitOfWorkContext;
        private readonly IOutbox _outbox;
        private readonly ReadOnlySettings _settings;
        private IOutboxTransaction _transaction;

        public UnitOfWorkManager(IDb db, IMessageSession messageSession, IUnitOfWorkContext unitOfWorkContext, IOutbox outbox, ReadOnlySettings settings)
        {
            _db = db;
            _messageSession = messageSession;
            _unitOfWorkContext = unitOfWorkContext;
            _outbox = outbox;
            _settings = settings;
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
                    var outboxMessage = events.Any() ? new OutboxMessage(GuidComb.NewGuidComb(), _settings.EndpointName(), events) : null;

                    if (outboxMessage != null)
                    {
                        _outbox.StoreAsync(outboxMessage, _transaction).GetAwaiter().GetResult();
                    }

                    _transaction.CommitAsync();

                    if (outboxMessage != null)
                    {
                        var options = new SendOptions();

                        options.RouteToThisEndpoint();
                        options.SetMessageId(outboxMessage.MessageId.ToString());

                        _messageSession.Send(new ProcessOutboxMessageCommand(), options).GetAwaiter().GetResult();
                    }
                }
            }
        }
    }
}