using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddAzureTableStorage(this IConfigurationBuilder builder, params string[] configurationKeys)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (configurationKeys == null || !configurationKeys.Any())
                throw new ArgumentException("At least one configuration key is required", nameof(configurationKeys));
            
            var environmentVariables = ConfigurationBootstrapper.GetEnvironmentVariables();

            var configOptions = new ConfigurationOptions
            {
                EnvironmentName = environmentVariables.EnvironmentName,
                TableStorageConnectionString = environmentVariables.TableStorageConnectionString,
                ConfigurationKeys = configurationKeys
            };

            var configurationSource = new AzureTableStorageConfigurationSource(configOptions);

            return builder.Add(configurationSource);
        }

        public static IConfigurationBuilder AddAzureTableStorage(this IConfigurationBuilder builder, Action<StorageOptions> setupOptions)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (setupOptions == null)
                throw new ArgumentNullException(nameof(setupOptions));

            var options = new StorageOptions
            {
                EnvironmentNameEnvironmentVariableName = EnvironmentVariableNames.EnvironmentName,
                StorageConnectionStringEnvironmentVariableName = EnvironmentVariableNames.ConfigurationStorageConnectionString
            };
            setupOptions(options);

            if (!options.ConfigurationKeys.Any())
                throw new ArgumentException("At least one configuration key is required", nameof(options.ConfigurationKeys));

            var configOptions = new ConfigurationOptions
            {
                ConfigurationKeys = options.ConfigurationKeys,
                PrefixConfigurationKeys = options.PreFixConfigurationKeys
            };

            configOptions.EnvironmentName = options.EnvironmentName ??
                ConfigurationBootstrapper.GetEnvironmentNameFromEnvironmentVariable(
                    options.EnvironmentNameEnvironmentVariableName);
            
            configOptions.TableStorageConnectionString = options.StorageConnectionString ??
                ConfigurationBootstrapper.GetConnectionStringFromEnvironmentVariable(
                    options.StorageConnectionStringEnvironmentVariableName, configOptions.EnvironmentName);

            var configurationSource = new AzureTableStorageConfigurationSource(configOptions);

            return builder.Add(configurationSource);
        }
    }
}