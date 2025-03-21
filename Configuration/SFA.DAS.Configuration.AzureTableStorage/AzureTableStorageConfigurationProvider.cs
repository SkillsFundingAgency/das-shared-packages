using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    public class AzureTableStorageConfigurationProvider : ConfigurationProvider
    {
        private const string ConfigurationTableName = "Configuration";
        private readonly string _configurationTableRowKeyVersion;

        private readonly string _environmentName;
        private readonly IEnumerable<string> _configurationKeys;
        private readonly bool _prefixConfigurationKeys;
        private readonly IEnumerable<string> _configurationKeysRawJsonResult;
        private readonly TableServiceClient _client;

        public AzureTableStorageConfigurationProvider(TableServiceClient tableClient ,string environmentName, IEnumerable<string> configurationKeys, bool prefixConfigurationKeys, IEnumerable<string> configurationKeysRawJsonResult, bool configurationNameIncludesVersionNumber)
        {
            _client = tableClient;
            _environmentName = environmentName;
            _configurationKeys = configurationKeys;
            _prefixConfigurationKeys = prefixConfigurationKeys;
            _configurationKeysRawJsonResult = configurationKeysRawJsonResult;
            _configurationTableRowKeyVersion = configurationNameIncludesVersionNumber ? "" : "1.0";
        }
        
        public override void Load()
        {
            var tableClient = _client.GetTableClient(ConfigurationTableName);
            var filter = $"PartitionKey eq '{_environmentName}' and (RowKey eq '{string.Join($"_{_configurationTableRowKeyVersion}' or RowKey eq '",_configurationKeys)}_{_configurationTableRowKeyVersion}')";
            var table = tableClient.QueryAsync<TableEntity>(filter: filter);
            var data = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            Task.WhenAll(_configurationKeys.Select(k => GetTableRowData(table, k, data))).ConfigureAwait(false).GetAwaiter().GetResult();
            
            Data = data;
        }
        
        private async Task GetTableRowData(AsyncPageable<TableEntity> table, string configurationName,
            ConcurrentDictionary<string, string> data)
        {
            var configParams = configurationName.Split(':');
            var configDefaultSectionName = configParams.Length > 1 ? configParams[1] : string.Empty;
            var configurationKey = configParams[0];

            var loadData = new ConcurrentDictionary<string, string>();
            
            await foreach (var item in table)
            {
                loadData.AddOrUpdate(item.RowKey, item["Data"].ToString(), (k, v) => item["Data"].ToString());
            }


            if (_configurationKeysRawJsonResult != null && _configurationKeysRawJsonResult.Contains(configurationKey))
            {
                var key = loadData.Keys.FirstOrDefault(c => c.Contains(configurationKey));
                if (key != null)
                {
                    data.AddOrUpdate(configurationKey, loadData[key], (k, v) => loadData[configurationKey]);
                }
                return;
            }

            foreach (var item in loadData)
            {

                var parsedData = JsonConfigurationStreamParser.Parse(item.Value.ToStream());

                foreach (var keyValuePair in parsedData)
                {
                    if (_prefixConfigurationKeys)
                    {
                        data.AddOrUpdate($"{configurationKey}:{keyValuePair.Key}", keyValuePair.Value,
                            (k, v) => keyValuePair.Value);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(configDefaultSectionName))
                        {
                            data.AddOrUpdate($"{keyValuePair.Key}", keyValuePair.Value, (k, v) => keyValuePair.Value);
                        }
                        else
                        {
                            data.AddOrUpdate($"{configDefaultSectionName}:{keyValuePair.Key}", keyValuePair.Value,
                                (k, v) => keyValuePair.Value);
                        }

                    }
                }
            }
        }
    
    }
}