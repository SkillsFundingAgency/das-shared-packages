
namespace SFA.DAS.Configuration
{
    public class ConfigurationOptions
    {
        public ConfigurationOptions(string serviceName, string environmentName, string versionNumber)
        {
            ServiceName = serviceName;
            EnvironmentName = environmentName;
            VersionNumber = versionNumber;
        }

        public string ServiceName { get; private set; }
        public string EnvironmentName { get; private set; }
        public string VersionNumber { get; private set; }
        
    }
}
