using StructureMap;

namespace SFA.DAS.UnitOfWork.NServiceBus
{
    public class NServiceBusUnitOfWorkRegistry : Registry
    {
        public NServiceBusUnitOfWorkRegistry()
        {
            For<IUnitOfWork>().Add<UnitOfWork>();
            For<IUnitOfWork>().Add<ClientOutbox.UnitOfWork>();
            For<IUnitOfWorkManager>().Add<ClientOutbox.UnitOfWorkManager>();
        }
    }
}