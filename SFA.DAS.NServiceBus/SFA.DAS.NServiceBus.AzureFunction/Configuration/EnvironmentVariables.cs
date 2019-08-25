using System;

namespace SFA.DAS.NServiceBus.AzureFunction.Configuration
{
    public static class EnvironmentVariables
    {
        public static readonly string NServiceBusConnectionString = Environment.GetEnvironmentVariable("NServiceBusConnectionString");
        public static readonly string NServiceBusLicense = Environment.GetEnvironmentVariable("NServiceBusLicense");
    }
}
