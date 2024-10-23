using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.NServiceBus.TestMessages.Events;
using Microsoft.Azure.Functions.Worker;

namespace SFA.DAS.NServiceBus.AzureFunctionExample
{
    public static class HandleSampleEvent
    {
        [Function(nameof(HandleSampleEvent))]
        public static Task Run([QueueTrigger(TestHarnessSettings.SampleQueueName)] SampleEvent message, ILogger log)
        {
            log.LogInformation($"Message '{message.GetType().Name}' received with '{nameof(message.Data)}' property value '{message.Data}'");

            return Task.CompletedTask;
        }
    }
}
