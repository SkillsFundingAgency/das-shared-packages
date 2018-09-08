using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Settings;
using SFA.DAS.NServiceBus.UnitOfWork;

namespace SFA.DAS.NServiceBus.ClientOutbox.UnitOfWork
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly IDb _db;
        private readonly IMessageSession _messageSession;
        private readonly IUnitOfWorkContext _unitOfWorkContext;
        private readonly IClientOutboxStorage _clientOutboxStorage;
        private readonly ReadOnlySettings _settings;
        private IClientOutboxTransaction _transaction;

        public UnitOfWorkManager(IDb db, IMessageSession messageSession, IUnitOfWorkContext unitOfWorkContext, IClientOutboxStorage clientOutboxStorage, ReadOnlySettings settings)
        {
            _db = db;
            _messageSession = messageSession;
            _unitOfWorkContext = unitOfWorkContext;
            _clientOutboxStorage = clientOutboxStorage;
            _settings = settings;
        }

        public void Begin()
        {
            _transaction = _clientOutboxStorage.BeginTransactionAsync().GetAwaiter().GetResult();
        }

        public void End(Exception ex = null)
        {
            using (_transaction)
            {
                if (ex == null)
                {
                    _db.SaveChangesAsync().GetAwaiter().GetResult();

                    var events = _unitOfWorkContext.GetEvents().ToList();
                    var clientOutboxMessage = events.Any() ? new ClientOutboxMessage(GuidComb.NewGuidComb(), _settings.EndpointName(), events) : null;

                    if (clientOutboxMessage != null)
                    {
                        _clientOutboxStorage.StoreAsync(clientOutboxMessage, _transaction).GetAwaiter().GetResult();
                    }

                    _transaction.CommitAsync().GetAwaiter().GetResult();

                    if (clientOutboxMessage != null)
                    {
                        var options = new SendOptions();

                        options.RouteToThisEndpoint();
                        options.SetMessageId(clientOutboxMessage.MessageId.ToString());

                        _messageSession.Send(new ProcessClientOutboxMessageCommand(), options).GetAwaiter().GetResult();
                    }
                }
            }
        }
    }
}