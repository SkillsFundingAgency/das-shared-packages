using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.AutoConfiguration
{
    public interface IAzureTableStorageConnectionAdapter
    {
        CloudTable GetTableReference(string storageConnectionString, string tableReference);

        TableOperation GetRetrieveOperation(string environmentName, string rowKey);

        TableResult Execute(CloudTable table, TableOperation operation);
    }
}