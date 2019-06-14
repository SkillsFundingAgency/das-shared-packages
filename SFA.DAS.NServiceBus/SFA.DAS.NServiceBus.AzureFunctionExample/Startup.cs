using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.NServiceBus.AzureFunctionExample;

[assembly: WebJobsStartup(typeof(Startup))]
namespace SFA.DAS.NServiceBus.AzureFunctionExample
{
    public class Startup :IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            //Environment variables need creating for
            //      NServiceBusConnectionString
            //      NServiceBusLicense
            builder.AddExecutionContextBinding();
            builder.AddExtension<NServiceBusExtensionConfig>();
        }
    }
}
