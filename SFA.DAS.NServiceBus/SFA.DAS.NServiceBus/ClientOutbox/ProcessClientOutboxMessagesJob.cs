using System.Linq;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.NServiceBus.ClientOutbox
{
    public class ProcessClientOutboxMessagesJob : IProcessClientOutboxMessagesJob
    {
        private readonly IMessageSession _messageSession;
        private readonly IClientOutboxStorage _clientOutboxStorage;
        private readonly IClientOutboxStorageV2 _clientOutboxStorageV2;

        public ProcessClientOutboxMessagesJob(IMessageSession messageSession, IClientOutboxStorage clientOutboxStorage, IClientOutboxStorageV2 clientOutboxStorageV2)
        {
            _messageSession = messageSession;
            _clientOutboxStorage = clientOutboxStorage;
            _clientOutboxStorageV2 = clientOutboxStorageV2;
        }

        public async Task RunAsync()
        {
            var clientOutboxMessagesTask = _clientOutboxStorage.GetAwaitingDispatchAsync();
            var clientOutboxMessageV2sTask = _clientOutboxStorageV2.GetAwaitingDispatchAsync();
            var clientOutboxMessages = await clientOutboxMessagesTask.ConfigureAwait(false);
            var clientOutboxMessageV2s = await clientOutboxMessageV2sTask.ConfigureAwait(false);
            
            var tasks = clientOutboxMessages
                .Select(m =>
                {
                    var sendOptions = new SendOptions();
                    
                    sendOptions.SetDestination(m.EndpointName);
                    sendOptions.SetMessageId(m.MessageId.ToString());
    
                    return _messageSession.Send(new ProcessClientOutboxMessageCommand(), sendOptions);
                })
                .Concat(clientOutboxMessageV2s.Select(m =>
                {
                    var sendOptions = new SendOptions();
                    
                    sendOptions.SetDestination(m.EndpointName);
                    sendOptions.SetMessageId(m.MessageId.ToString());
    
                    return _messageSession.Send(new ProcessClientOutboxMessageCommandV2(), sendOptions);
                }));

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}