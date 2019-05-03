using SFA.DAS.NServiceBus;
using SFA.DAS.UnitOfWork.StructureMap;
using StructureMap;

namespace SFA.DAS.UnitOfWork.NServiceBus.ClientOutbox.StructureMap
{
    public class NServiceBusClientUnitOfWorkRegistry : Registry
    {
        public NServiceBusClientUnitOfWorkRegistry()
        {
            IncludeRegistry<UnitOfWorkRegistry>();
            For<IEventPublisher>().Use<EventPublisher>();
            For<IUnitOfWork>().Add<UnitOfWork>();
            For<IUnitOfWorkManager>().Add<UnitOfWorkManager>();
        }
    }
}