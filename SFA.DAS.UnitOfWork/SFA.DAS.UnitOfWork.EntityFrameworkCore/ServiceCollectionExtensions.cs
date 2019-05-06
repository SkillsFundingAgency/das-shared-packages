using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.UnitOfWork.EntityFrameworkCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityFrameworkUnitOfWork<T>(this IServiceCollection services) where T : DbContext
        {
            return services.AddScoped<IUnitOfWork, UnitOfWork<T>>()
                .AddScoped(s => new Lazy<T>(s.GetService<T>));
        }
    }
}