using System;
using System.Reflection;

namespace SFA.DAS.Configuration
{
    public class ConfigurationOptions
    {
        public ConfigurationOptions(string serviceName = null, string environmentName = null, string versionNumber = null)
        {
            ServiceName = string.IsNullOrEmpty(serviceName) ? Assembly.GetEntryAssembly().GetName().Name : serviceName;
            EnvironmentName = string.IsNullOrEmpty(environmentName) ? GetEnvironmentName() : environmentName;
            VersionNumber = string.IsNullOrEmpty(serviceName) ? GetVersionNumer() : versionNumber;
        }

        public string ServiceName { get; private set; }
        public string EnvironmentName { get; private set; }
        public string VersionNumber { get; private set; }

        private string GetEnvironmentName()
        {
            var environmentName = Environment.GetEnvironmentVariable("DASENV");
            return string.IsNullOrEmpty(environmentName) ? "Dev" : environmentName;
        }

        private string GetVersionNumer()
        {
            var version = Assembly.GetEntryAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}";
        }
    }
}
