using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.UnitOfWork
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            return services.AddScoped<IUnitOfWorkContext, UnitOfWorkContext>()
                .AddScoped<IUnitOfWorkScope, UnitOfWorkScope>();
        }
    }
}