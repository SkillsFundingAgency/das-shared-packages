using System;

namespace SFA.DAS.NServiceBus.ClientOutbox
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