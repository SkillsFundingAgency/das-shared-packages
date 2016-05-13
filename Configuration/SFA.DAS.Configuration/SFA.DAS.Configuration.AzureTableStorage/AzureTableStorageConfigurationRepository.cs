using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    public class AzureTableStorageConfigurationRepository : IConfigurationRepository
    {
        private CloudStorageAccount _storageAccount;

        public AzureTableStorageConfigurationRepository()
            : this(CloudConfigurationManager.GetSetting("StorageConnectionString"))
        {
        }
        public AzureTableStorageConfigurationRepository(string storageConnectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(storageConnectionString);
        }

        public async Task<string> Get(string serviceName, string environmentName, string version)
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("Configuration");

            var tableOperation = TableOperation.Retrieve<ConfigurationItem>(environmentName, $"{serviceName}_{version}");
            var result = await table.ExecuteAsync(tableOperation);

            var configItem = (ConfigurationItem)result.Result;
            return configItem == null ? null : configItem.Data;
        }
    }
}
