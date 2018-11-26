using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.NServiceBus.ClientOutbox.Commands;

namespace SFA.DAS.NServiceBus.ClientOutbox
{
    public class ProcessClientOutboxMessagesJob : IProcessClientOutboxMessagesJob
    {
        private readonly IMessageSession _messageSession;
        private readonly IClientOutboxStorage _clientOutboxStorage;

        public ProcessClientOutboxMessagesJob(IMessageSession messageSession, IClientOutboxStorage clientOutboxStorage)
        {
            _messageSession = messageSession;
            _clientOutboxStorage = clientOutboxStorage;
        }

        public async Task RunAsync()
        {
            var clientOutboxMessages = await _clientOutboxStorage.GetAwaitingDispatchAsync();

            var tasks = clientOutboxMessages.Select(m =>
            {
                var options = new SendOptions();
                
                options.SetDestination(m.EndpointName);
                options.SetMessageId(m.MessageId.ToString());

                return _messageSession.Send(new ProcessClientOutboxMessageCommand(), options);
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}