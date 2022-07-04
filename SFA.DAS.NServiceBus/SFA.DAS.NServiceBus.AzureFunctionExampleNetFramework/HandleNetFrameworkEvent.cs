using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using SFA.DAS.NServiceBus.NetStandardMessages.Events;

namespace SFA.DAS.NServiceBus.AzureFunctionExampleNetFramework
{
    public class HandleNetFrameworkEvent
    {
        [FunctionName(nameof(HandleNetFrameworkEvent))]
        public static Task Run([NServiceBusTrigger(Endpoint = "SFA.DAS.NServiceBus.NetFrameworkEndpoint")] NetFrameworkEvent message, ILogger log)
        {
            log.LogInformation($"Message '{message.GetType().Name}' received with '{nameof(message.Data)}' property value '{message.Data}'");

            return Task.CompletedTask;
        }
    }
}
