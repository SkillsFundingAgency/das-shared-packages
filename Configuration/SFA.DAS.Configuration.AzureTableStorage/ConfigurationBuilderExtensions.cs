using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddAzureTableStorageConfiguration(this IConfigurationBuilder builder,
            string connectionString, string environmentName, IEnumerable<string> configurationKeys)
        {
            return builder.Add(new AzureTableStorageConfigurationSource(connectionString, environmentName, configurationKeys));
        }
    }
}