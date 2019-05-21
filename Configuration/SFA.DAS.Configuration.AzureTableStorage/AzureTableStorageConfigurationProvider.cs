using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    public class AzureTableStorageConfigurationProvider : ConfigurationProvider
    {
        private const string ConfigurationTableName = "Configuration";
        private const string ConfigurationTableRowKeyVersion = "1.0";

        private readonly CloudStorageAccount _storageAccount;
        private readonly string _environmentName;
        private readonly IEnumerable<string> _configurationKeys;
        private readonly bool _prefixConfigurationKeys;

        public AzureTableStorageConfigurationProvider(CloudStorageAccount cloudStorageAccount, string environmentName, IEnumerable<string> configurationKeys, bool prefixConfigurationKeys)
        {
            _storageAccount = cloudStorageAccount;
            _environmentName = environmentName;
            _configurationKeys = configurationKeys;
            _prefixConfigurationKeys = prefixConfigurationKeys;
        }
        
        public override void Load()
        {
            var client = _storageAccount.CreateCloudTableClient();
            var table = client.GetTableReference(ConfigurationTableName);
            var data = new ConcurrentDictionary<string, string>();
            var tasks = _configurationKeys.Select(k => GetTableRowData(table, k, data));
            
            Task.WhenAll(tasks).ConfigureAwait(false).GetAwaiter().GetResult();
            
            Data = data;
        }

        private async Task GetTableRowData(CloudTable table, string configurationKey, ConcurrentDictionary<string, string> data)
        {
            var operation = GetTableRowOperation(configurationKey);
            var row = await table.ExecuteAsync(operation).ConfigureAwait(false);
            var configurationRow = (ConfigurationRow)row.Result;

            using (var stream = configurationRow.Data.ToStream())
            {
                var parsedData = JsonConfigurationStreamParser.Parse(stream);

                foreach (var keyValuePair in parsedData)
                {
                    if (_prefixConfigurationKeys)
                    {
                        data.AddOrUpdate($"{configurationKey}:{keyValuePair.Key}", keyValuePair.Value, (k, v) => keyValuePair.Value);
                    }
                    else
                    {
                        data.AddOrUpdate($"{keyValuePair.Key}", keyValuePair.Value, (k, v) => keyValuePair.Value);
                    }
                }
            }
        }
        
        protected virtual TableOperation GetTableRowOperation(string configurationKey)
        {
            return TableOperation.Retrieve<ConfigurationRow>(_environmentName, $"{configurationKey}_{ConfigurationTableRowKeyVersion}");
        }
        
        internal interface IConfigurationRow : ITableEntity
        {
            string Data { get; set; }
        }
        
        internal class ConfigurationRow : TableEntity, IConfigurationRow
        {
            public string Data { get; set; }
        }
    }
}