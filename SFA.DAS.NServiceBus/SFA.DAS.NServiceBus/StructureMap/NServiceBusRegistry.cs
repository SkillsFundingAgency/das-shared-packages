using StructureMap;

namespace SFA.DAS.NServiceBus.StructureMap
{
    public class NServiceBusRegistry : Registry
    {
        public NServiceBusRegistry()
        {
            For<IProcessOutboxMessagesJob>().Use<ProcessOutboxMessagesJob>();
        }
    }
}