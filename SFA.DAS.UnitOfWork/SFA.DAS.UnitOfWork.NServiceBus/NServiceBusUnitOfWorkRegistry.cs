using StructureMap;

namespace SFA.DAS.UnitOfWork.NServiceBus
{
    public class NServiceBusUnitOfWorkRegistry : Registry
    {
        public NServiceBusUnitOfWorkRegistry()
        {
            IncludeRegistry<UnitOfWorkRegistry>();
            For<IUnitOfWork>().Add<UnitOfWork>();
        }
    }
}