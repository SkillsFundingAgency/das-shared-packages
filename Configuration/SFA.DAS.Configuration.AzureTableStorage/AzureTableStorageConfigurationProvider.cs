using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SFA.DAS.Configuration.Extensions;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    public class AzureTableStorageConfigurationProvider : ConfigurationProvider
    {
        // das's tools (das-employer-config) don't currently support different versions, so might as well hardcode it
        // we provide versioning by appending a 'Vn' on to the name
        private const string Version = "1.0";
        private const string ConfigurationTableName = "Configuration";
        
        private readonly IEnumerable<string> _configKeys;
        private readonly string _environment;
        private readonly CloudStorageAccount _storageAccount;

        public AzureTableStorageConfigurationProvider(CloudStorageAccount cloudStorageAccount, string environment, IEnumerable<string> configKeys)
        {
            _configKeys = configKeys;
            _environment = environment;
            _storageAccount = cloudStorageAccount;
        }

        internal interface IConfigurationRow : ITableEntity
        {
            string Data { get; set; }
        }
        
        internal class ConfigurationRow : TableEntity, IConfigurationRow
        {
            public string Data { get; set; }
        }
        
        public override void Load()
        {
            Data = ReadAndParseConfigurationTableRows().GetAwaiter().GetResult();
        }
        
        //pass instance props in??
        private async Task<ConcurrentDictionary<string, string>> ReadAndParseConfigurationTableRows()
        {
            var concurrentData = new ConcurrentDictionary<string, string>();

            var parseRowsTasks = _configKeys.Select(configKey => ParseConfigurationTableRow(concurrentData, configKey));

            await Task.WhenAll(parseRowsTasks).ConfigureAwait(false);
            
            return concurrentData;
        }
        
        private async Task ParseConfigurationTableRow(ConcurrentDictionary<string, string> data, string configKey)
        {
            string config = await GetRowConfiguration(configKey).ConfigureAwait(false);

            using (var stream = config.ToStream())
            {
                var configData = JsonConfigurationStreamParser.Parse(stream);

                foreach (var configItem in configData)
                    data.AddOrUpdate($"{configKey}:{configItem.Key}", configItem.Value, (key, oldValue) => configItem.Value);
            }
        }

        //combine next 2?
        private async Task<string> GetRowConfiguration(string configKey)
        {
            var tableResult = await GetTableResult(GetTable(), configKey).ConfigureAwait(false);
            return ((ConfigurationRow) tableResult.Result).Data;
        }

        private async Task<TableResult> GetTableResult(CloudTable table, string configKey)
        {
            return await table.ExecuteAsync(GetOperation(configKey)).ConfigureAwait(false);
        }

        private CloudTable GetTable()
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            return tableClient.GetTableReference(ConfigurationTableName);
        }

        /// <remarks>
        /// protected virtual so can create a derived object and override for unit testing
        /// bit of a hack, until MS update fakes for core, or they release a easily unit testable library
        /// alternative is to introduce an injected class to provide a level of indirection
        /// </remarks>
        protected virtual TableOperation GetOperation(string configKey)
        {
            return TableOperation.Retrieve<ConfigurationRow>(_environment, $"{configKey}_{Version}");
        }
    }
}