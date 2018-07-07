using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.NServiceBus
{
    public class ProcessOutboxMessageCommandHandler : IHandleMessages<ProcessOutboxMessageCommand>
    {
        private readonly IOutbox _outbox;

        public ProcessOutboxMessageCommandHandler(IOutbox outbox)
        {
            _outbox = outbox;
        }

        public async Task Handle(ProcessOutboxMessageCommand message, IMessageHandlerContext context)
        {
            var outboxMessageId = Guid.Parse(context.MessageId);
            var outboxMessage = await _outbox.GetById(outboxMessageId);
            var events = outboxMessage.Publish();
            var tasks = events.Select(context.Publish);

            await Task.WhenAll(tasks);
        }
    }
}