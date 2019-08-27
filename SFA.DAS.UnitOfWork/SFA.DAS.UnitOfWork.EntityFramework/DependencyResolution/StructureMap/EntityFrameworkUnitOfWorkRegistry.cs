using System.Data.Entity;
using SFA.DAS.UnitOfWork.EntityFramework.Pipeline;
using SFA.DAS.UnitOfWork.Pipeline;
using StructureMap;

namespace SFA.DAS.UnitOfWork.EntityFramework.StructureMap
{
    public class EntityFrameworkUnitOfWorkRegistry<T> : Registry where T : DbContext
    {
        public EntityFrameworkUnitOfWorkRegistry()
        {
            For<IUnitOfWork>().Add<UnitOfWork<T>>();
        }
    }
}