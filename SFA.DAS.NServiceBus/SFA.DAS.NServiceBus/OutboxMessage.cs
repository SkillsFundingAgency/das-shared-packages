using System;
using System.Collections.Generic;

namespace SFA.DAS.NServiceBus
{
    public class OutboxMessage : IOutboxMessageAwaitingDispatch
    {
        public Guid MessageId { get; }
        public string EndpointName { get; }
        public IEnumerable<Event> Operations { get; } = new List<Event>();

        public OutboxMessage(Guid messageId, string endpointName)
        {
            MessageId = messageId;
            EndpointName = endpointName;
        }

        public OutboxMessage(Guid messageId, string endpointName, IEnumerable<Event> operations)
        {
            MessageId = messageId;
            EndpointName = endpointName;
            Operations = operations;
        }
    }
}