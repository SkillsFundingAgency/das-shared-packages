namespace SFA.DAS.Configuration.AzureTableStorage
{
    public class StorageOptions
    {
        /// <summary>
        /// Get EnvironmentName from the value of this given environment variable.
        /// </summary>
        public string EnvironmentNameEnvironmentVariableName { get; set; }
        /// <summary>
        /// Get StorageConnectionString from the value of this given environment variable.
        /// </summary>
        public string StorageConnectionStringEnvironmentVariableName { get; set; }
        /// <summary>
        /// Directly supply EnvironmentName (overrides getting value from environment variable specified by EnvironmentNameEnvironmentVariableName).
        /// </summary>
        public string EnvironmentName { get; set; }
        /// <summary>
        /// Directly supply StorageConnectionString (overrides getting value from environment variable specified by StorageConnectionStringEnvironmentVariableName).
        /// </summary>
        public string StorageConnectionString { get; set; }
        public string[] ConfigurationKeys { get; set; }
        public bool PreFixConfigurationKeys { get; set; } = true;
        public string[] ConfigurationKeysRawJsonResult { get ; set ; } = null;
    }
}