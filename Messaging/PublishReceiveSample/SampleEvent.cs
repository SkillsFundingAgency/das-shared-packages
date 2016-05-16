using System;

namespace PublishReceiveSample
{
    public class SampleEvent
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Id { get; set; }
    }
}
