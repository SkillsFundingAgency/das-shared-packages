using StructureMap;

namespace SFA.DAS.UnitOfWork.NServiceBus.ClientOutbox
{
    public class NServiceBusUnitOfWorkRegistry : Registry
    {
        public NServiceBusUnitOfWorkRegistry()
        {
            For<IUnitOfWork>().Add<UnitOfWork>();
            For<IUnitOfWorkManager>().Add<UnitOfWorkManager>();
        }
    }
}