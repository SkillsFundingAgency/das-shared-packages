﻿namespace SFA.DAS.Configuration
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

        private string GetServiceName()
        {
            return GetAssemblyName().Name;
        }
        private string GetEnvironmentName()
        {
            var environmentName = Environment.GetEnvironmentVariable("DASENV");
            return string.IsNullOrEmpty(environmentName) ? "Dev" : environmentName;
        }
        private string GetVersionNumer()
        {
            var version = GetAssemblyName().Version;
            return $"{version.Major}.{version.Minor}";
        }
        private AssemblyName GetAssemblyName()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                throw new InvalidOperationException("Unable to determine assembly");
            }

            var assemblyName = assembly.GetName();
            if (assemblyName == null)
            {
                throw new InvalidOperationException("Unable to determine assembly name");
            }

            return assemblyName;
        }
    }
}
