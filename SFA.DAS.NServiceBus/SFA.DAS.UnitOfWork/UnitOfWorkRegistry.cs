using StructureMap;

namespace SFA.DAS.UnitOfWork
{
    public class UnitOfWorkRegistry : Registry
    {
        public UnitOfWorkRegistry()
        {
            For<IUnitOfWorkContext>().Use<UnitOfWorkContext>();
        }
    }
}