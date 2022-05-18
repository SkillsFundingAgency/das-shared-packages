using System;

using System.Linq;
#if NET462
using System.Configuration;
using Microsoft.Azure;
#endif

namespace SFA.DAS.AutoConfiguration
{
    public class EnvironmentService : IEnvironmentService
    {
        private const string Prefix = "APPSETTING_";
        private const string EnvironmentName = "EnvironmentName";
        private const string DefaultEnvironment = "LOCAL";

        public string GetVariable(string variableName, string environmentType = "")
        {
#if NET462
            if (environmentType.Equals("app_service"))
            {
                return ConfigurationManager.AppSettings[variableName];
            }
            else if (environmentType.Equals("app_service_env"))
            {
                return Environment.GetEnvironmentVariable(Prefix + variableName);    
            }
            return CloudConfigurationManager.GetSetting(variableName);
#else    
            return Environment.GetEnvironmentVariable(Prefix + variableName);
#endif
        }

        public bool IsCurrent(params DasEnv[] environment)
        {
            return environment.Any(x => x == (DasEnv)Enum.Parse(typeof(DasEnv), GetVariable(EnvironmentName) ?? DefaultEnvironment));
        }
    }
}