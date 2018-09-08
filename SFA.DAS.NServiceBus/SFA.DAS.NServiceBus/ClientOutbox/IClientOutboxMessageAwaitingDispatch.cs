using System;

namespace SFA.DAS.NServiceBus.ClientOutbox
{
    public interface IClientOutboxMessageAwaitingDispatch
    {
        Guid MessageId { get; }
        string EndpointName { get; }
    }
}