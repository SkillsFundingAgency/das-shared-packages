using System;

namespace SFA.DAS.Events.Api.Types
{
    public interface IGenericEvent<T> : IEventView
    {
        DateTime CreatedOn { get; set; }
        T Payload { get; set; }
        string ResourceUri { get; set; }
        string ResourceType { get; set; }
        string ResourceId { get; set; }
    }
}
