using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    /// <remarks>
    /// Inspired by...
    /// https://github.com/SkillsFundingAgency/das-reservations/blob/MF-7-reservations-web/src/SFA.DAS.Reservations.Infrastructure/Configuration/AzureTableStorageConfigurationProvider.cs
    /// </remarks>
    public class AzureTableStorageConfigurationProvider : ConfigurationProvider
    {
        // das's tools (das-employer-config) don't currently support different versions, so might as well hardcode it
        // we provider versioning by appending a 'Vn' on to the name
        private const string Version = "1.0";
        private const string ConfigurationTableName = "Configuration";
        private readonly IEnumerable<string> _configNames;
        private readonly string _environment;
        private readonly CloudStorageAccount _storageAccount;
        //todo: would be better to create jit
        private readonly ConcurrentDictionary<string, string> _concurrentData;

        public AzureTableStorageConfigurationProvider(CloudStorageAccount cloudStorageAccount, string environment, IEnumerable<string> configNames)
        {
            _configNames = configNames;
            _environment = environment;
            _storageAccount = cloudStorageAccount;
            _concurrentData = new ConcurrentDictionary<string, string>();
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
                configStreams = configJsons.Select(GenerateStreamFromString);

                var configNameAndStreams = _configNames.Zip(configStreams, (name, stream) => (name, stream));

                Parallel.ForEach(configNameAndStreams, AddToData);
                Data = _concurrentData;
            }
            finally
            {
                foreach (var stream in configStreams)
                {
                    stream.Dispose();
                }
            }
        }

        /// <returns>Stream that contains the supplied string. The caller is responsible for disposing the stream.</returns>
        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
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
            var tableOperation = GetOperation(serviceName);
            return table.ExecuteAsync(tableOperation);
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
        
        private void AddToData((string name, Stream stream) configNameAndStream)
        {
            var configData = JsonConfigurationStreamParser.Parse(configNameAndStream.stream);

            foreach (var configItem in configData)
                _concurrentData.AddOrUpdate($"{configNameAndStream.name}:{configItem.Key}", configItem.Value, (key, oldValue) => configItem.Value);
        }
    }
}