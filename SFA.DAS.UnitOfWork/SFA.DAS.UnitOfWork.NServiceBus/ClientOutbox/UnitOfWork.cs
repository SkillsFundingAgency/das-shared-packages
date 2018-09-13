using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Settings;
using NServiceBus.UniformSession;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.UnitOfWork.NServiceBus.ClientOutbox
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IUniformSession _uniformSession;
        private readonly IUnitOfWorkContext _unitOfWorkContext;
        private readonly IClientOutboxStorage _clientOutboxStorage;
        private readonly ReadOnlySettings _settings;

        public UnitOfWork(IClientOutboxStorage clientOutboxStorage, IUniformSession uniformSession, IUnitOfWorkContext unitOfWorkContext, ReadOnlySettings settings)
        {
            _clientOutboxStorage = clientOutboxStorage;
            _uniformSession = uniformSession;
            _unitOfWorkContext = unitOfWorkContext;
            _settings = settings;
        }

        public async Task CommitAsync(Func<Task> next)
        {
            var clientOutboxTransaction = _unitOfWorkContext.Get<IClientOutboxTransaction>();
            var events = _unitOfWorkContext.GetEvents().ToList();
            var clientOutboxMessage = events.Any() ? new ClientOutboxMessage(GuidComb.NewGuidComb(), _settings.EndpointName(), events) : null;

            if (clientOutboxMessage != null)
            {
                await _clientOutboxStorage.StoreAsync(clientOutboxMessage, clientOutboxTransaction).ConfigureAwait(false);
            }

            await next().ConfigureAwait(false);

            if (clientOutboxMessage != null)
            {
                var options = new SendOptions();

                options.RouteToThisEndpoint();
                options.SetMessageId(clientOutboxMessage.MessageId.ToString());

                await _uniformSession.Send(new ProcessClientOutboxMessageCommand(), options).ConfigureAwait(false);
            }
        }
    }
}