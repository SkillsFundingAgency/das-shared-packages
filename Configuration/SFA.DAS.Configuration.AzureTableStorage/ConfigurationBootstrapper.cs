using System;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    internal static class ConfigurationBootstrapper
    {
        private const string DeveloperEnvironmentName = "LOCAL";
        private const string DeveloperEnvironmentDefaultConnectionString = "UseDevelopmentStorage=true";

        public static EnvironmentVariables GetEnvironmentVariables()
        {
            return GetRequiredEnvironmentVariables(EnvironmentVariableNames.ConfigurationStorageConnectionString, EnvironmentVariableNames.EnvironmentName);
        }

        public static EnvironmentVariables GetEnvironmentVariables(string connectionStringKey, string environmentNameKey)
        {
            return GetRequiredEnvironmentVariables(connectionStringKey, environmentNameKey);
        }

        private static EnvironmentVariables GetRequiredEnvironmentVariables(string connectionStringKey, string environmentNameKey)
        {
            var environmentName = Environment.GetEnvironmentVariable(environmentNameKey) ?? DeveloperEnvironmentName;
            var connectionString = Environment.GetEnvironmentVariable(connectionStringKey);
            
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

            return new EnvironmentVariables(connectionString, environmentName);
        }
    }
}