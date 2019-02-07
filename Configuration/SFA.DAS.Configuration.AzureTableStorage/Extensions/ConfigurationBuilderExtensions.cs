using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Configuration.AzureTableStorage.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddAzureTableStorageConfiguration(this IConfigurationBuilder builder,
            string connectionString, string environmentName, params string[] configurationKeys)
        {
            return builder.Add(new AzureTableStorageConfigurationSource(connectionString, environmentName, configurationKeys));
        }
        
        public static IConfigurationBuilder AddAzureTableStorageConfiguration(this IConfigurationBuilder builder, params string[] configurationKeys)
        {
            var environmentVariables = ConfigurationBootstrapper.GetEnvironmentVariables();

            return AddAzureTableStorageConfiguration(builder, environmentVariables.StorageConnectionString,
                environmentVariables.EnvironmentName, configurationKeys);
        }
    }
}