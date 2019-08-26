using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.UnitOfWork.EntityFrameworkCore.Pipeline;
using SFA.DAS.UnitOfWork.Pipeline;

namespace SFA.DAS.UnitOfWork.EntityFrameworkCore.DependencyResolution.Microsoft
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