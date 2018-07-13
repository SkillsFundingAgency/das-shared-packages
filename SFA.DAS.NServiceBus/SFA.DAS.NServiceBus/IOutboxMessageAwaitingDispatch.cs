using System;

namespace SFA.DAS.NServiceBus
{
    public interface IOutboxMessageAwaitingDispatch
    {
        Guid MessageId { get; }
        string EndpointName { get; }
    }
}