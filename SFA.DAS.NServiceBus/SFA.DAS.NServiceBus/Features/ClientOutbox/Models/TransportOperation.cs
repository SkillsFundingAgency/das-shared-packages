using System;

// ReSharper disable once CheckNamespace - Required for backwards compatibility with SFA.DAS.NServiceBus < 14.0.0
namespace SFA.DAS.NServiceBus.Features.ClientOutbox.Models
{
    public class TransportOperation
    {
        public Guid MessageId { get; }
        public object Message { get; }

        public TransportOperation(Guid messageId, object message)
        {
            MessageId = messageId;
            Message = message;
        }
    }
}