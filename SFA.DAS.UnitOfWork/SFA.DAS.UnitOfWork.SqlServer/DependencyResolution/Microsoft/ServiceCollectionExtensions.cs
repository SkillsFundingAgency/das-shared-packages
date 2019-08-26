using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.UnitOfWork.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.Managers;
using SFA.DAS.UnitOfWork.SqlServer.Managers;

namespace SFA.DAS.UnitOfWork.SqlServer.DependencyResolution.Microsoft
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlServerUnitOfWork(this IServiceCollection services)
        {
            return services.AddUnitOfWork()
                .AddScoped<IUnitOfWorkManager, UnitOfWorkManager>();
        }
    }
}