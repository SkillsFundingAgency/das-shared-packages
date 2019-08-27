using System;
using System.Collections.Generic;

namespace SFA.DAS.NServiceBus.Features.ClientOutbox.Models
{
    public class ClientOutboxMessageV2 : IClientOutboxMessageAwaitingDispatch
    {
        public Guid MessageId { get; }
        public string EndpointName { get; }
        public IEnumerable<TransportOperation> TransportOperations { get; } = new List<TransportOperation>();

        public ClientOutboxMessageV2(Guid messageId, string endpointName)
        {
            MessageId = messageId;
            EndpointName = endpointName;
        }

        public ClientOutboxMessageV2(Guid messageId, string endpointName, IEnumerable<TransportOperation> transportOperations)
        {
            MessageId = messageId;
            EndpointName = endpointName;
            TransportOperations = transportOperations;
        }
    }
}