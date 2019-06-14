using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    public class AzureTableStorageConfigurationSource : IConfigurationSource
    {
        private readonly string _connectionString;
        private readonly string _environmentName;
        private readonly IEnumerable<string> _configurationKeys;
        private readonly bool _prefixConfigurationKeys;

        public AzureTableStorageConfigurationSource(ConfigurationOptions configOptions)
        {
            _connectionString = configOptions.TableStorageConnectionString;
            _environmentName = configOptions.EnvironmentName;
            _configurationKeys = configOptions.ConfigurationKeys;
            _prefixConfigurationKeys = configOptions.PrefixConfigurationKeys;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AzureTableStorageConfigurationProvider(CloudStorageAccount.Parse(_connectionString), _environmentName, _configurationKeys, _prefixConfigurationKeys);
        }
    }
}