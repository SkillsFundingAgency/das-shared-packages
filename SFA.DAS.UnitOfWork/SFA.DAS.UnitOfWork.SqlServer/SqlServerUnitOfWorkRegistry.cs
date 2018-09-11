using StructureMap;

namespace SFA.DAS.UnitOfWork.SqlServer
{
    public class SqlServerUnitOfWorkRegistry : Registry
    {
        public SqlServerUnitOfWorkRegistry()
        {
            For<IUnitOfWorkManager>().Add<UnitOfWorkManager>();
        }
    }
}