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

        public AzureTableStorageConfigurationSource(string connectionString, string environmentName, IEnumerable<string> configurationKeys)
        {
            _connectionString = connectionString;
            _environmentName = environmentName;
            _configurationKeys = configurationKeys;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AzureTableStorageConfigurationProvider(CloudStorageAccount.Parse(_connectionString), _environmentName, _configurationKeys);
        }
    }
}