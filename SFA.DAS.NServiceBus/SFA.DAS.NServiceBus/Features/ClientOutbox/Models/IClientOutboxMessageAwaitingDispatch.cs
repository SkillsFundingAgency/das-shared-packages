using System;

namespace SFA.DAS.NServiceBus.Features.ClientOutbox.Models
{
    public interface IClientOutboxMessageAwaitingDispatch
    {
        Guid MessageId { get; }
        string EndpointName { get; }
    }
}