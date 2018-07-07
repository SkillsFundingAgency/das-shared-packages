using System.Linq;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.NServiceBus
{
    public class ProcessOutboxMessagesJob : IProcessOutboxMessagesJob
    {
        private readonly IMessageSession _messageSession;
        private readonly IOutbox _outbox;

        public ProcessOutboxMessagesJob(IMessageSession messageSession, IOutbox outbox)
        {
            _messageSession = messageSession;
            _outbox = outbox;
        }

        public async Task RunAsync()
        {
            var outboxMessageIds = await _outbox.GetIdsToProcess();

            var tasks = outboxMessageIds.Select(i =>
            {
                var options = new SendOptions();
                
                options.SetMessageId(i.ToString());

                return _messageSession.Send(new ProcessOutboxMessageCommand(), options);
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}