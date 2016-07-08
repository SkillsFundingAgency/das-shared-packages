using System;

namespace PublishReceiveSample
{
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
