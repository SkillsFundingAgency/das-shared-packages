using System;

namespace SFA.DAS.Events.Api.Types
{
    public class GenericEvent  : IEventView
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
        public string ResourceType { get; set; }
        public string ResourceId { get; set; }
    }
}
