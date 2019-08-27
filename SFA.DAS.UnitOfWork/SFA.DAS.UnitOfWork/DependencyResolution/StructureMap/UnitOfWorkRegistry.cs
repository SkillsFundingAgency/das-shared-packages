using SFA.DAS.UnitOfWork.Context;
using StructureMap;

namespace SFA.DAS.UnitOfWork.DependencyResolution.StructureMap
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