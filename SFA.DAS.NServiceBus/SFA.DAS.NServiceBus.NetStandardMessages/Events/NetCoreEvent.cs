namespace SFA.DAS.NServiceBus.NetStandardMessages.Events
{
    public class NetCoreEvent
    {
        public string Data { get; }

        public NetCoreEvent(string data)
        {
            Data = data;
        }
    }
}