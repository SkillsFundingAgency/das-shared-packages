using System.Data.Entity;
using StructureMap;

namespace SFA.DAS.UnitOfWork.EntityFramework
{
    public class EntityFrameworkUnitOfWorkRegistry<T> : Registry where T : DbContext
    {
        public EntityFrameworkUnitOfWorkRegistry()
        {
            For<IUnitOfWork>().Add<UnitOfWork<T>>();
        }
    }
}