using SFA.DAS.NServiceBus.ClientOutbox;
using StructureMap;

namespace SFA.DAS.NServiceBus.StructureMap
{
    public class NServiceBusRegistry : Registry
    {
        public NServiceBusRegistry()
        {
            For<IProcessClientOutboxMessagesJob>().Use<ProcessClientOutboxMessagesJob>();
        }
    }
}