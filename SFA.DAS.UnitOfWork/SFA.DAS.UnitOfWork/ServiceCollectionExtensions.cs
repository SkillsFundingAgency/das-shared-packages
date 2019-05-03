using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SFA.DAS.UnitOfWork
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.TryAddScoped<IUnitOfWorkContext, UnitOfWorkContext>();
            services.TryAddScoped<IUnitOfWorkScope, UnitOfWorkScope>();

            return services;
        }
    }
}