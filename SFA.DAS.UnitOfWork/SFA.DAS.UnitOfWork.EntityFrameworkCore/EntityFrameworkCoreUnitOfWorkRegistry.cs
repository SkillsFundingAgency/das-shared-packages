using Microsoft.EntityFrameworkCore;
using StructureMap;

namespace SFA.DAS.UnitOfWork.EntityFrameworkCore
{
    public class EntityFrameworkCoreUnitOfWorkRegistry<T> : Registry where T : DbContext
    {
        public EntityFrameworkCoreUnitOfWorkRegistry()
        {
            For<IUnitOfWork>().Add<UnitOfWork<T>>();
        }
    }
}