using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddAzureTableStorageConfiguration(this IConfigurationBuilder builder, string connection, string environment, IEnumerable<string> configNames)
        {
            return builder.Add(new AzureTableStorageConfigurationSource(connection, environment, configNames));
        }
    }
}