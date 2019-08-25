using System;
using System.Collections.Generic;

namespace SFA.DAS.NServiceBus.Features.ClientOutbox.Models
{
    public class ClientOutboxMessage : IClientOutboxMessageAwaitingDispatch
    {
        public Guid MessageId { get; }
        public string EndpointName { get; }
        public IEnumerable<object> Operations { get; } = new List<object>();

        public ClientOutboxMessage(Guid messageId, string endpointName)
        {
            MessageId = messageId;
            EndpointName = endpointName;
        }

        public ClientOutboxMessage(Guid messageId, string endpointName, IEnumerable<object> operations)
        {
            MessageId = messageId;
            EndpointName = endpointName;
            Operations = operations;
        }
    }
}