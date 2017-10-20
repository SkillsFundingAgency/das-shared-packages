using System;
using SFA.DAS.Messaging.Attributes;

namespace PublishReceiveSample
{
    [MessageGroup("sample-event")]
    public class SampleEvent
    {
        public SampleEvent()
        {
            Id = Guid.NewGuid().ToString();
            Timestamp = DateTimeOffset.Now;
        }
        public DateTimeOffset Timestamp { get; set; }
        public string Id { get; set; }
    }
}
