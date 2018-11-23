using System;

namespace SFA.DAS.AutoConfiguration
{
    public class EnvironmentService : IEnvironmentService
    {
        private const string Prefix = "AppSettings_";

        public string GetVariable(string variableName)
        {
            return Environment.GetEnvironmentVariable(Prefix + variableName);
        }
    }
}