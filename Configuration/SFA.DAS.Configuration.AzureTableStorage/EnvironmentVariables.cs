namespace SFA.DAS.Configuration.AzureTableStorage
{
    public class EnvironmentVariables
    {
        public EnvironmentVariables(string tableStorageConnectionString, string environmentName)
        {
            TableStorageConnectionString = tableStorageConnectionString;
            EnvironmentName = environmentName;
        }

        public string TableStorageConnectionString { get; set; }
        public string EnvironmentName { get; set; }
    }
}