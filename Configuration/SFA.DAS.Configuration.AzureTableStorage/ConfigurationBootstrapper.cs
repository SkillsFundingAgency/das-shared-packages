using System;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    internal static class ConfigurationBootstrapper
    {
        private const string DeveloperEnvironment = "LOCAL";
        private const string DeveloperEnvironmentDefaultConnectionString = "UseDevelopmentStorage=true";

        public static (string ConnectionString, string EnvironmentName) GetEnvironmentVariables()
        {
            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var environmentName = configuration[EnvironmentVariableNames.Environment] ?? DeveloperEnvironment;
            var connectionString = configuration[EnvironmentVariableNames.ConfigurationStorageConnectionString];
            
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                if (string.Equals(environmentName, DeveloperEnvironment, StringComparison.OrdinalIgnoreCase))
                {
                    connectionString = DeveloperEnvironmentDefaultConnectionString;
                }
                else
                {
                    throw new Exception($"Missing environment variable '{EnvironmentVariableNames.ConfigurationStorageConnectionString}'. It should be present and set to a connection string pointing to the storage account containing a 'Configuration' table.");
                }
            }

            return (connectionString, environmentName);
        }
    }
}