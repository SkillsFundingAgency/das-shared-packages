using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;
using SFA.DAS.NServiceBus.NetStandardMessages.Events;

namespace SFA.DAS.NServiceBus.AzureFunctionExample
{
    public class HandleEvent
    {
        [FunctionName("HandleSignedAgreementEvent")]
        public static async Task Run([NServiceBusTrigger(EndPoint = "SFA.DAS.NServiceBus.AzureFunctionExample")] NetFrameworkEvent message, ILogger log)
        {
            log.LogInformation($"Message '{message.GetType().Name}' received with '{nameof(message.Data)}' property value '{message.Data}'");
            
        }
    }
}
