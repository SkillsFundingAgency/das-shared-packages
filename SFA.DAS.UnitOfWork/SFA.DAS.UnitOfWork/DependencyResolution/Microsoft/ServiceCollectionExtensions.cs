using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.UnitOfWork.DependencyResolution.Microsoft
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