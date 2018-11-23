using System;
using System.Linq;

namespace SFA.DAS.AutoConfiguration
{
    public class EnvironmentService : IEnvironmentService
    {
        private const string Prefix = "AppSettings_";
        private const string EnvironmentName = "EnvironmentName";

        public string GetVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(Prefix + variableName);
        }

        public bool IsCurrent(params DasEnv[] environment)
        {
            return environment.Any(x => x == (DasEnv)Enum.Parse(typeof(DasEnv), GetVariable(EnvironmentName)));
        }
    }
}