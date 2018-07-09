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
            var outboxMessage = await _outbox.GetAsync(outboxMessageId);
            var tasks = outboxMessage.Operations.Select(context.Publish);

            await Task.WhenAll(tasks);
            await _outbox.SetAsDispatchedAsync(outboxMessage.MessageId);
        }
    }
}