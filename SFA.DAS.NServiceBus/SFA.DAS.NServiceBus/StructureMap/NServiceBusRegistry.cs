using StructureMap;

namespace SFA.DAS.NServiceBus.StructureMap
{
    public class NServiceBusRegistry : Registry
    {
        public NServiceBusRegistry()
        {
            For<IEventPublisher>().Use<EventPublisher>();
            For<IProcessOutboxMessagesJob>().Use<ProcessOutboxMessagesJob>();
            For<IUnitOfWorkContext>().Use<UnitOfWorkContext>();
            For<IUnitOfWorkManager>().Use<UnitOfWorkManager>();
        }
    }
}