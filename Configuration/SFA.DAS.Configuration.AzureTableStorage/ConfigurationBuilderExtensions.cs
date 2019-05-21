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

            var configOptions = new ConfigurationOptions
            {
                EnvironmentVariableKeys = environmentVariables,
                ConfigurationKeys = configurationKeys
            };

            var configurationSource = new AzureTableStorageConfigurationSource(configOptions);

            return builder.Add(configurationSource);
        }

        public static IConfigurationBuilder AddAzureTableStorage(this IConfigurationBuilder builder, Action<StorageOptions> setupOptions)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupOptions == null)
            {
                throw new ArgumentNullException(nameof(setupOptions));
            }

            var options = new StorageOptions();
            setupOptions.Invoke(options);
            
            if (options == null || !options.ConfigurationKeys.Any())
            {
                throw new ArgumentException("At least one configuration key is required", nameof(options.ConfigurationKeys));
            }

            var environmentNameKey = string.IsNullOrWhiteSpace(options.EnvironmentNameEnvironmentVariableName) ? EnvironmentVariableNames.EnvironmentName : options.EnvironmentNameEnvironmentVariableName;
            var storageConnectionStringKey = string.IsNullOrWhiteSpace(options.StorageConnectionStringEnvironmentVariableName) ? EnvironmentVariableNames.ConfigurationStorageConnectionString : options.StorageConnectionStringEnvironmentVariableName;

            var environmentVariables = ConfigurationBootstrapper.GetEnvironmentVariables(storageConnectionStringKey, environmentNameKey);

            var configOptions = new ConfigurationOptions
            {
                EnvironmentVariableKeys = environmentVariables,
                ConfigurationKeys = options.ConfigurationKeys,
                PrefixConfigurationKeys = options.PreFixConfigurationKeys
            };

            var configurationSource = new AzureTableStorageConfigurationSource(configOptions);
            
            return builder.Add(configurationSource);
        }
    }
}