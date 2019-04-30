using StructureMap;

namespace SFA.DAS.UnitOfWork.StructureMap
{
    public class UnitOfWorkRegistry : Registry
    {
        public UnitOfWorkRegistry()
        {
            For<IUnitOfWorkContext>().Use<UnitOfWorkContext>();
            For<IUnitOfWorkScope>().Use<UnitOfWorkScope>();
        }
    }
}