using SFA.DAS.UnitOfWork.StructureMap;
using StructureMap;

namespace SFA.DAS.UnitOfWork.SqlServer.StructureMap
{
    public class SqlServerUnitOfWorkRegistry : Registry
    {
        public SqlServerUnitOfWorkRegistry()
        {
            IncludeRegistry<UnitOfWorkRegistry>();
            For<IUnitOfWorkManager>().Add<UnitOfWorkManager>();
        }
    }
}