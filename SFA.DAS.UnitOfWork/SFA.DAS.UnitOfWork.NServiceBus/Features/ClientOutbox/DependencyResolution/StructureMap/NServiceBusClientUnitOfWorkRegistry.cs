using SFA.DAS.NServiceBus.Services;
using SFA.DAS.UnitOfWork.DependencyResolution.StructureMap;
using SFA.DAS.UnitOfWork.Managers;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.Managers;
using SFA.DAS.UnitOfWork.NServiceBus.Services;
using SFA.DAS.UnitOfWork.Pipeline;
using StructureMap;

namespace SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.DependencyResolution.StructureMap
{
    public class NServiceBusClientUnitOfWorkRegistry : Registry
    {
        public NServiceBusClientUnitOfWorkRegistry()
        {
            IncludeRegistry<UnitOfWorkRegistry>();
            For<IEventPublisher>().Use<EventPublisher>();
            For<IUnitOfWork>().Add<Pipeline.UnitOfWork>();
            For<IUnitOfWorkManager>().Add<UnitOfWorkManager>();
        }
    }
}