using SFA.DAS.Messaging.Attributes;

namespace PubSubSampleGui
{
    [MessageGroup("Sample")]
    public class SampleMessage
    {
        public string Id { get; set; }
        public decimal Balance { get; set; }
    }
}