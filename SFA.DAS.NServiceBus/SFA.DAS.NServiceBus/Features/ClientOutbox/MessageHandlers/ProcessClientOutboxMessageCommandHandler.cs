using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;

namespace SFA.DAS.NServiceBus.Features.ClientOutbox.MessageHandlers
{
    public class ProcessClientOutboxMessageCommandHandler : IHandleMessages<ProcessClientOutboxMessageCommand>
    {
        private readonly IClientOutboxStorage _clientOutboxStorage;

        public ProcessClientOutboxMessageCommandHandler(IClientOutboxStorage clientOutboxStorage)
        {
            _clientOutboxStorage = clientOutboxStorage;
        }

        public async Task Handle(ProcessClientOutboxMessageCommand message, IMessageHandlerContext context)
        {
            var clientOutboxMessageId = Guid.Parse(context.MessageId);
            var clientOutboxMessage = await _clientOutboxStorage.GetAsync(clientOutboxMessageId, context.SynchronizedStorageSession);

            await Task.WhenAll(clientOutboxMessage.Operations.Select(context.Publish));
            await _clientOutboxStorage.SetAsDispatchedAsync(clientOutboxMessage.MessageId, context.SynchronizedStorageSession);
        }
    }
}