using System;

namespace SFA.DAS.Events.Api.Types
{
    public interface IGenericEvent<T> : IEventView
    {
        DateTime CreatedOn { get; }
        T Payload { get; }
        string ResourceType { get; }
        string ResourceId { get; }
    }
}
