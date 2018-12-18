using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace SFA.DAS.AutoConfiguration
{
    public class TableStorageConfigurationService : IAutoConfigurationService
    {
        private const string ConfigurationTableReference = "Configuration";
        private const string DefaultVersion = "1.0";
        private const string DefaultEnvironment = "LOCAL";
        private const string DefaultStorageConnectionString = "UseDevelopmentStorage=true";
        private readonly IEnvironmentService _environmentService;
        private readonly IAzureTableStorageConnectionAdapter _azureTableStorageConnectionAdapter;
        

        public TableStorageConfigurationService(IEnvironmentService environmentService, IAzureTableStorageConnectionAdapter azureTableStorageConnectionAdapter)
        {
            _environmentService = environmentService;
            _azureTableStorageConnectionAdapter = azureTableStorageConnectionAdapter;
        }

        public T Get<T>()
        {
            return Get<T>(null);
        }

        public T Get<T>(string rowKey)
        {
            var environmentName = _environmentService.GetVariable(EnvironmentVariableNames.Environment) ?? DefaultEnvironment;
            var storageConnectionString = _environmentService.GetVariable(EnvironmentVariableNames.ConfigurationStorageConnectionString) ?? DefaultStorageConnectionString;
            rowKey = rowKey == null ? $"{Assembly.GetAssembly(typeof(T)).GetName().Name}_{DefaultVersion}" : $"{rowKey}_{DefaultVersion}";

            var table = _azureTableStorageConnectionAdapter.GetTableReference(storageConnectionString, ConfigurationTableReference);

            var operation = _azureTableStorageConnectionAdapter.GetRetrieveOperation(environmentName, rowKey);
            TableResult result;
            try
            {
                result = _azureTableStorageConnectionAdapter.Execute(table, operation);
            }
            catch (Exception e)
            {
                throw new Exception("Could not connect to Storage to retrieve settings.", e);
            }

            var dynResult = result.Result as DynamicTableEntity;
            var data = dynResult.Properties["Data"].StringValue;

            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
