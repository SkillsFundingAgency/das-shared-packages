using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Settings;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.UnitOfWork.NServiceBus.ClientOutbox
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IClientOutboxStorageV2 _clientOutboxStorage;
        private readonly IMessageSession _messageSession;
        private readonly IUnitOfWorkContext _unitOfWorkContext;
        private readonly ReadOnlySettings _settings;

        public UnitOfWork(IClientOutboxStorageV2 clientOutboxStorage, IMessageSession messageSession, IUnitOfWorkContext unitOfWorkContext, ReadOnlySettings settings)
        {
            _clientOutboxStorage = clientOutboxStorage;
            _messageSession = messageSession;
            _unitOfWorkContext = unitOfWorkContext;
            _settings = settings;
        }

        public async Task CommitAsync(Func<Task> next)
        {
            var clientOutboxTransaction = _unitOfWorkContext.Get<IClientOutboxTransaction>();
            var transportOperations = _unitOfWorkContext.GetEvents().Select(e => new TransportOperation(GuidComb.NewGuidComb(), e)).ToList();
            var clientOutboxMessage = transportOperations.Any() ? new ClientOutboxMessageV2(GuidComb.NewGuidComb(), _settings.EndpointName(), transportOperations) : null;

            if (clientOutboxMessage != null)
            {
                await _clientOutboxStorage.StoreAsync(clientOutboxMessage, clientOutboxTransaction).ConfigureAwait(false);
            }

            await next().ConfigureAwait(false);

            if (clientOutboxMessage != null)
            {
                var tasks = clientOutboxMessage.TransportOperations.Select(o => 
                {
                    var publishOptions = new PublishOptions();
                
                    publishOptions.SetMessageId(o.MessageId.ToString());

                    return _messageSession.Publish(o.Message, publishOptions);
                });
            
                await Task.WhenAll(tasks).ConfigureAwait(false);
                await _clientOutboxStorage.SetAsDispatchedAsync(clientOutboxMessage.MessageId).ConfigureAwait(false);
            }
        }
    }
}