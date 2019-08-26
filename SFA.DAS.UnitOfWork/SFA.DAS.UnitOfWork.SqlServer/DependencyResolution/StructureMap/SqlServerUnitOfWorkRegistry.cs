using SFA.DAS.UnitOfWork.DependencyResolution.StructureMap;
using SFA.DAS.UnitOfWork.Managers;
using SFA.DAS.UnitOfWork.SqlServer.Managers;
using StructureMap;

namespace SFA.DAS.UnitOfWork.SqlServer.DependencyResolution.StructureMap
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