using SFA.DAS.NServiceBus.Services;
using SFA.DAS.UnitOfWork.StructureMap;
using StructureMap;

namespace SFA.DAS.UnitOfWork.NServiceBus.StructureMap
{
    public class NServiceBusUnitOfWorkRegistry : Registry
    {
        public NServiceBusUnitOfWorkRegistry()
        {
            IncludeRegistry<UnitOfWorkRegistry>();
            For<IEventPublisher>().Use<EventPublisher>();
            For<IUnitOfWork>().Add<UnitOfWork>();
        }
    }
}