using System;
using System.Reflection;

namespace SFA.DAS.Configuration
{
    public class ConfigurationOptions
    {
        public ConfigurationOptions(string serviceName = null, string environmentName = null, string versionNumber = null)
        {
            var assemblyName = Assembly.GetEntryAssembly().GetName();

            ServiceName = string.IsNullOrEmpty(serviceName) ? assemblyName.Name : serviceName;
            EnvironmentName = string.IsNullOrEmpty(environmentName) ? GetEnvironmentName() : environmentName;
            VersionNumber = string.IsNullOrEmpty(serviceName) ? $"{assemblyName.Version.Major}.{assemblyName.Version.Minor}" : versionNumber;
        }

        public string ServiceName { get; private set; }
        public string EnvironmentName { get; private set; }
        public string VersionNumber { get; private set; }

        private string GetEnvironmentName()
        {
            var environmentName = Environment.GetEnvironmentVariable("DASENV");
            return string.IsNullOrEmpty(environmentName) ? "Dev" : environmentName;
        }
    }
}
