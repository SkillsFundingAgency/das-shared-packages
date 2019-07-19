using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.AutoConfiguration.DependencyResolution
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoConfiguration(this IServiceCollection services)
        {
            return services.AddTransient<IAutoConfigurationService, TableStorageConfigurationService>()
                .AddTransient<IEnvironmentService, EnvironmentService>()
                .AddTransient<IAzureTableStorageConnectionAdapter, AzureTableStorageConnectionAdapter>();
        }
    }
}