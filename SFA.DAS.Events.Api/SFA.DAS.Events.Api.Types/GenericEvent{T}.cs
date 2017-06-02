namespace SFA.DAS.Events.Api.Types
{
    using System;

    public class GenericEvent<T> : IEventView
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public new string Type => typeof(T).FullName;
        public T Payload { get; set; }
        public string ResourceUri { get; set; }
        public string ResourceType { get; set; }
        public string ResourceId { get; set; }
    }
}
