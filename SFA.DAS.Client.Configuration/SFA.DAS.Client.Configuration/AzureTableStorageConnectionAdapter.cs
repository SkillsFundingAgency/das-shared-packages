using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.Client.Configuration
{
    public class AzureTableStorageConnectionAdapter : IAzureTableStorageConnectionAdapter
    {
        public CloudTable GetTableReference(string storageConnectionString, string tableReference)
        {
            var conn = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = conn.CreateCloudTableClient();
            return tableClient.GetTableReference(tableReference);
        }

        public TableOperation GetRetrieveOperation(string environmentName, string rowKey)
        {
            return TableOperation.Retrieve(environmentName, rowKey);
        }

        public TableResult Execute(CloudTable table, TableOperation operation)
        {
            return table.ExecuteAsync(operation).GetAwaiter().GetResult();
        }
    }
}