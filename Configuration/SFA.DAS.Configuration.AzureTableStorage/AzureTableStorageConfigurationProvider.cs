using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
        
        private readonly IEnumerable<string> _configNames;
        private readonly string _environment;
        private readonly CloudStorageAccount _storageAccount;

        public AzureTableStorageConfigurationProvider(CloudStorageAccount cloudStorageAccount, string environment, IEnumerable<string> configNames)
        {
            _configNames = configNames;
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
            var configJsons = GetRows().Select(r => ((ConfigurationRow)r.Result).Data);

            IEnumerable<Stream> configStreams = null;
            try
            {
                configStreams = configJsons.Select(cj => cj.ToStream());

                var configNameAndStreams = _configNames.Zip(configStreams, (name, stream) => (name, stream));

                Data = ParseConfigurationTableRows(configNameAndStreams);
            }
            finally
            {
                foreach (var stream in configStreams)
                {
                    stream.Dispose();
                }
            }
        }
        
        private CloudTable GetTable()
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            return tableClient.GetTableReference(ConfigurationTableName);
        }

        private IEnumerable<TableResult> GetRows()
        {
            var table = GetTable();
            var operations = _configNames.Select(name => GetTableResult(table, name));
            return Task.WhenAll(operations).GetAwaiter().GetResult();
        }

        private Task<TableResult> GetTableResult(CloudTable table, string serviceName)
        {
            return table.ExecuteAsync(GetOperation(serviceName));
        }
        
        /// <remarks>
        /// protected virtual so can create a derived object and override for unit testing
        /// bit of a hack, until MS update fakes for core, or they release a easily unit testable library
        /// alternative is to introduce an injected class to provide a level of indirection
        /// </remarks>
        protected virtual TableOperation GetOperation(string serviceName)
        {
            return TableOperation.Retrieve<ConfigurationRow>(_environment, $"{serviceName}_{Version}");
        }
        
        private ConcurrentDictionary<string, string> ParseConfigurationTableRows(IEnumerable<(string name, Stream stream)> configNameAndStream)
        {
            var concurrentData = new ConcurrentDictionary<string, string>();

            Parallel.ForEach(configNameAndStream, r => ParseConfigurationTableRow(concurrentData, r));

            return concurrentData;
        }

        //todo: how much can we parallelize?
        private void ParseConfigurationTableRow(ConcurrentDictionary<string, string> data, (string name, Stream stream) configNameAndStream)
        {
            var configData = JsonConfigurationStreamParser.Parse(configNameAndStream.stream);

            foreach (var configItem in configData)
                data.AddOrUpdate($"{configNameAndStream.name}:{configItem.Key}", configItem.Value, (key, oldValue) => configItem.Value);
        }
    }
}