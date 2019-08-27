using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;

namespace SFA.DAS.NServiceBus.Features.ClientOutbox.MessageHandlers
{
    public class ProcessClientOutboxMessageCommandV2Handler : IHandleMessages<ProcessClientOutboxMessageCommandV2>
    {
        private readonly IClientOutboxStorageV2 _clientOutboxStorage;

        public ProcessClientOutboxMessageCommandV2Handler(IClientOutboxStorageV2 clientOutboxStorage)
        {
            _clientOutboxStorage = clientOutboxStorage;
        }

        public async Task Handle(ProcessClientOutboxMessageCommandV2 message, IMessageHandlerContext context)
        {
            var clientOutboxMessageId = Guid.Parse(context.MessageId);
            var clientOutboxMessage = await _clientOutboxStorage.GetAsync(clientOutboxMessageId, context.SynchronizedStorageSession);

            var tasks = clientOutboxMessage.TransportOperations.Select(o =>
            {
                var publishOptions = new PublishOptions();
                
                publishOptions.SetMessageId(o.MessageId.ToString());

                return context.Publish(o.Message, publishOptions);
            });
            
            await Task.WhenAll(tasks);
            await _clientOutboxStorage.SetAsDispatchedAsync(clientOutboxMessage.MessageId, context.SynchronizedStorageSession);
        }
    }
}