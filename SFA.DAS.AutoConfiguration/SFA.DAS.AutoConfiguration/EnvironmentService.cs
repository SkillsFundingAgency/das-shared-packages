using System;

using System.Linq;
#if NET462
using System.Configuration;
#endif

namespace SFA.DAS.AutoConfiguration
{
    public class EnvironmentService : IEnvironmentService
    {
        private const string Prefix = "APPSETTING_";
        private const string EnvironmentName = "EnvironmentName";
        private const string DefaultEnvironment = "LOCAL";

        public string GetVariable(string variableName)
        {
            
#if NET462
            return ConfigurationManager.AppSettings[variableName];
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