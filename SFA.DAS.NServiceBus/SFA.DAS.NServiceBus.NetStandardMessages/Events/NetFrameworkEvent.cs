namespace SFA.DAS.NServiceBus.NetStandardMessages.Events
{
    public class NetFrameworkEvent
    {
        public string Data { get; }

        public NetFrameworkEvent(string data)
        {
            Data = data;
        }
    }
}