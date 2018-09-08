using System;
using System.Collections.Generic;

namespace SFA.DAS.NServiceBus.ClientOutbox
{
    public class ClientOutboxMessage : IClientOutboxMessageAwaitingDispatch
    {
        public Guid MessageId { get; }
        public string EndpointName { get; }
        public IEnumerable<Event> Operations { get; } = new List<Event>();

        public ClientOutboxMessage(Guid messageId, string endpointName)
        {
            MessageId = messageId;
            EndpointName = endpointName;
        }

        public ClientOutboxMessage(Guid messageId, string endpointName, IEnumerable<Event> operations)
        {
            MessageId = messageId;
            EndpointName = endpointName;
            Operations = operations;
        }
    }
}