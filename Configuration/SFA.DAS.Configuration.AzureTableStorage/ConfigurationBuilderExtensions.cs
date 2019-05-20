using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    public class ConfigurationOptions
    {
        public EnvironmentVariables EnvironmentVariableKeys { get; set; }
        public string[] ConfigurationKeys { get; set; }
        public bool PrefixConfigurationKeys { get; set; } = true;
    }

    public class EnvironmentVariables
    {
        public EnvironmentVariables(string tableStorageConnectionString, string environmentName)
        {
            TableStorageConnectionString = tableStorageConnectionString;
            EnvironmentName = environmentName;
        }

        public string TableStorageConnectionString { get; set; }
        public string EnvironmentName { get; set; }
    }


    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddAzureTableStorage(this IConfigurationBuilder builder, params string[] configurationKeys)
        {
            if (configurationKeys == null || !configurationKeys.Any())
            {
                throw new ArgumentException("At least one configuration key is required", nameof(configurationKeys));
            }
            
            var (ConnectionStringKey, EnvironmentNameKey) = ConfigurationBootstrapper.GetEnvironmentVariables();

            var configOptions = new ConfigurationOptions
            {
                EnvironmentVariableKeys = new EnvironmentVariables(ConnectionStringKey, EnvironmentNameKey),
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

            var (ConnectionStringKey, EnvironmentNameKey) = ConfigurationBootstrapper.GetEnvironmentVariables(storageConnectionStringKey, environmentNameKey);

            var configOptions = new ConfigurationOptions
            {
                EnvironmentVariableKeys = new EnvironmentVariables(ConnectionStringKey, EnvironmentNameKey),
                ConfigurationKeys = options.ConfigurationKeys,
                PrefixConfigurationKeys = options.PreFixConfigurationKeys
            };

            var configurationSource = new AzureTableStorageConfigurationSource(configOptions);
            
            return builder.Add(configurationSource);
        }
    }
}