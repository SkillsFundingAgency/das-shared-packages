using StructureMap;

namespace SFA.DAS.UnitOfWork.SqlServer
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