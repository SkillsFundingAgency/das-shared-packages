using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.UnitOfWork.SqlServer
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