using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.NServiceBus.AzureFunction.Infrastructure
{
    public class EnvironmentVariables
    {
        public static string NServiceBusConnectionString = Environment.GetEnvironmentVariable("NServiceBusConnectionString");
        public static string NServiceBusLicense = Environment.GetEnvironmentVariable("NServiceBusLicense");
    }
}
