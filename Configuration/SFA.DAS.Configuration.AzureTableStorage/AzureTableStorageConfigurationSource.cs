using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    public class AzureTableStorageConfigurationSource : IConfigurationSource
    {
        internal readonly string ConnectionString;
        internal readonly string EnvironmentName;
        internal readonly IEnumerable<string> ConfigurationKeys;
        internal readonly IEnumerable<string> ConfigurationKeysRawJsonResult;
        internal readonly bool PrefixConfigurationKeys;


        public AzureTableStorageConfigurationSource(ConfigurationOptions configOptions)
        {
            ConnectionString = configOptions.TableStorageConnectionString;
            EnvironmentName = configOptions.EnvironmentName;
            ConfigurationKeys = configOptions.ConfigurationKeys;
            PrefixConfigurationKeys = configOptions.PrefixConfigurationKeys;
            ConfigurationKeysRawJsonResult = configOptions.ConfigurationKeysRawJsonResult;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AzureTableStorageConfigurationProvider(CloudStorageAccount.Parse(ConnectionString), EnvironmentName, ConfigurationKeys, PrefixConfigurationKeys, ConfigurationKeysRawJsonResult);
        }
    }
}