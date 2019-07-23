using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SFA.DAS.AutoConfiguration.DependencyResolution
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoConfiguration(this IServiceCollection services)
        {
            services.TryAddTransient<IAutoConfigurationService, TableStorageConfigurationService>();
            services.TryAddTransient<IEnvironmentService, EnvironmentService>();
            services.TryAddTransient<IAzureTableStorageConnectionAdapter, AzureTableStorageConnectionAdapter>();
            
            return services;
        }
    }
}