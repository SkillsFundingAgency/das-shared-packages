using System;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    internal static class ConfigurationBootstrapper
    {
        private const string DeveloperEnvironmentName = "LOCAL";
        private const string DeveloperEnvironmentDefaultConnectionString = "UseDevelopmentStorage=true";

        public static (string ConnectionString, string EnvironmentName) GetEnvironmentVariables()
        {
            var environmentName = Environment.GetEnvironmentVariable(EnvironmentVariableNames.EnvironmentName) ?? DeveloperEnvironmentName;
            var connectionString = Environment.GetEnvironmentVariable(EnvironmentVariableNames.ConfigurationStorageConnectionString);
            
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                if (string.Equals(environmentName, DeveloperEnvironmentName, StringComparison.OrdinalIgnoreCase))
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