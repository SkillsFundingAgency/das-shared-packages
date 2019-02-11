using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddAzureTableStorage(this IConfigurationBuilder builder, params string[] configurationKeys)
        {
            if (configurationKeys == null || !configurationKeys.Any())
            {
                throw new ArgumentException("At least one configuration key is required", nameof(configurationKeys));
            }
            
            var environmentVariables = ConfigurationBootstrapper.GetEnvironmentVariables();
            var configurationSource = new AzureTableStorageConfigurationSource(environmentVariables.ConnectionString, environmentVariables.EnvironmentName, configurationKeys);
            
            return builder.Add(configurationSource);
        }
    }
}