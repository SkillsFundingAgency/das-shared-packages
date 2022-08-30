namespace SFA.DAS.NServiceBus.TestMessages.Events
{
    public class SampleEvent
    {
        public string Data { get; }

        public SampleEvent(string data)
        {
            Data = data;
        }
    }
}