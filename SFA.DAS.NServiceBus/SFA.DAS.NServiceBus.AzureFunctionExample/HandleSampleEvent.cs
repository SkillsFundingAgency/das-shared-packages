using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using SFA.DAS.NServiceBus.TestMessages.Events;

namespace SFA.DAS.NServiceBus.AzureFunctionExample
{
    public static class HandleSampleEvent
    {
        [FunctionName(nameof(HandleSampleEvent))]
        public static Task Run([NServiceBusTrigger(Endpoint = TestHarnessSettings.SampleQueueName)] SampleEvent message, ILogger log)
        {
            log.LogInformation($"Message '{message.GetType().Name}' received with '{nameof(message.Data)}' property value '{message.Data}'");

            return Task.CompletedTask;
        }
    }
}
