using SFA.DAS.NServiceBus.Services;
using SFA.DAS.UnitOfWork.DependencyResolution.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus.Services;
using SFA.DAS.UnitOfWork.Pipeline;
using StructureMap;

namespace SFA.DAS.UnitOfWork.NServiceBus.DependencyResolution.StructureMap
{
    public class NServiceBusUnitOfWorkRegistry : Registry
    {
        public NServiceBusUnitOfWorkRegistry()
        {
            IncludeRegistry<UnitOfWorkRegistry>();
            For<IEventPublisher>().Use<EventPublisher>();
            For<IUnitOfWork>().Add<Pipeline.UnitOfWork>();
        }
    }
}