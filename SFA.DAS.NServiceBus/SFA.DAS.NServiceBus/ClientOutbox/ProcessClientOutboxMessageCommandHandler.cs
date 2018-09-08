using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.NServiceBus.ClientOutbox
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
            var clientOutboxMessage = await _clientOutboxStorage.GetAsync(clientOutboxMessageId);
            var tasks = clientOutboxMessage.Operations.Select(context.Publish);

            await Task.WhenAll(tasks);
            await _clientOutboxStorage.SetAsDispatchedAsync(clientOutboxMessage.MessageId);
        }
    }
}