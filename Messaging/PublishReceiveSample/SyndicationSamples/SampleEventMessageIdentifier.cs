using SFA.DAS.Messaging.Syndication;

namespace PublishReceiveSample.SyndicationSamples
{
    internal class SampleEventMessageIdentifier : IMessageIdentifier<SampleEvent>
    {
        public string GetIdentifier(SampleEvent message)
        {
            return message.Id;
        }
    }
}