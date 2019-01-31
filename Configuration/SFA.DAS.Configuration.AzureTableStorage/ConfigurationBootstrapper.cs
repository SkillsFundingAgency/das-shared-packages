using System;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    public class ConfigurationBootstrapper
    {
        private const string DefaultEnvironment = "LOCAL";

        public static (string StorageConnectionString, string EnvironmentName) GetEnvironmentVariables()
        {
            var environmentVariablesConfig = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            
            var storageConnectionString = environmentVariablesConfig[EnvironmentVariableNames.ConfigurationStorageConnectionString];
            if (string.IsNullOrWhiteSpace(storageConnectionString))
                throw new Exception($"Hey {environmentVariablesConfig["USERNAME"]??"developer"}, you need to set the environment variable '{EnvironmentVariableNames.ConfigurationStorageConnectionString}'. Set it to a connection string pointing to a storage account containing a 'Configuration' table. See readme.md for more information.");

            var environmentName = environmentVariablesConfig[EnvironmentVariableNames.Environment] ?? DefaultEnvironment;

            return (storageConnectionString, environmentName);
        }

        public static IConfigurationRoot GetConfiguration(string storageConnectionString, string environmentName, params string[] configurationKeys)
        {
            return new ConfigurationBuilder().AddAzureTableStorageConfiguration(
                storageConnectionString, environmentName, configurationKeys).Build();
        }
        
        public static IConfigurationRoot GetConfiguration(params string[] configurationKeys)
        {
            var environmentVariables = GetEnvironmentVariables();

            return GetConfiguration(environmentVariables.StorageConnectionString, environmentVariables.EnvironmentName, configurationKeys);
        }
    }
}