namespace SFA.DAS.Configuration.AzureTableStorage
{
    public class ConfigurationOptions
    {
        public EnvironmentVariables EnvironmentVariableKeys { get; set; }
        public string[] ConfigurationKeys { get; set; }
        public bool PrefixConfigurationKeys { get; set; } = true;
    }
}