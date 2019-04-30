using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.UnitOfWork.EntityFrameworkCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkCoreUnitOfWork<T>(this IServiceCollection services) where T : DbContext
        {
            return services.AddScoped<IUnitOfWork, UnitOfWork<T>>();
        }
    }
}