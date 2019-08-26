using Microsoft.EntityFrameworkCore;
using SFA.DAS.UnitOfWork.EntityFrameworkCore.Pipeline;
using SFA.DAS.UnitOfWork.Pipeline;
using StructureMap;

namespace SFA.DAS.UnitOfWork.EntityFrameworkCore.DependencyResolution.StructureMap
{
    public class EntityFrameworkCoreUnitOfWorkRegistry<T> : Registry where T : DbContext
    {
        public EntityFrameworkCoreUnitOfWorkRegistry()
        {
            For<IUnitOfWork>().Add<UnitOfWork<T>>();
        }
    }
}