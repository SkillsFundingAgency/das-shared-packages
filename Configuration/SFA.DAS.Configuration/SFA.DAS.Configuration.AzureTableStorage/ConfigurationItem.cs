using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    internal class ConfigurationItem : TableEntity
    {
        public string Data { get; set; }
    }
}