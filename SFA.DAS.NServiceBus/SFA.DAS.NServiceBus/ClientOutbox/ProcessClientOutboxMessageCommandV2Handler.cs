using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.NServiceBus.ClientOutbox
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