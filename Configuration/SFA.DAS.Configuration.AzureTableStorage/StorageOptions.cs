namespace SFA.DAS.Configuration.AzureTableStorage
{
    public class StorageOptions
    {
        public string EnvironmentNameEnvironmentVariableName { get; set; }
        public string StorageConnectionStringEnvironmentVariableName { get; set; }
        public string[] ConfigurationKeys { get; set; }
        public bool PreFixConfigurationKeys { get; set; } = true;
    }
}