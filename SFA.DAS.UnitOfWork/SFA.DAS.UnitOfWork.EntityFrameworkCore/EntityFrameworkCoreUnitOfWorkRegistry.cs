using Microsoft.EntityFrameworkCore;
using StructureMap;

namespace SFA.DAS.UnitOfWork.EntityFrameworkCore
{
    public class EntityFrameworkUnitOfWorkRegistry<T> : Registry where T : DbContext
    {
        public EntityFrameworkUnitOfWorkRegistry()
        {
            For<IUnitOfWork>().Add<UnitOfWork<T>>();
        }
    }
}