using System.Collections.Generic;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    public class AzureTableStorageConfigurationSource : IConfigurationSource
    {
        internal readonly string ConnectionString;
        internal readonly string EnvironmentName;
        internal readonly IEnumerable<string> ConfigurationKeys;
        internal readonly IEnumerable<string> ConfigurationKeysRawJsonResult;
        internal readonly bool PrefixConfigurationKeys;
        internal readonly bool ConfigurationNameIncludesVersionNumber;


        public AzureTableStorageConfigurationSource(ConfigurationOptions configOptions)
        {
            ConnectionString = configOptions.TableStorageConnectionString;
            EnvironmentName = configOptions.EnvironmentName;
            ConfigurationKeys = configOptions.ConfigurationKeys;
            PrefixConfigurationKeys = configOptions.PrefixConfigurationKeys;
            ConfigurationKeysRawJsonResult = configOptions.ConfigurationKeysRawJsonResult;
            ConfigurationNameIncludesVersionNumber = configOptions.ConfigurationNameIncludesVersionNumber;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AzureTableStorageConfigurationProvider( new TableServiceClient(ConnectionString), EnvironmentName, ConfigurationKeys, PrefixConfigurationKeys, ConfigurationKeysRawJsonResult, ConfigurationNameIncludesVersionNumber);
        }
    }
}