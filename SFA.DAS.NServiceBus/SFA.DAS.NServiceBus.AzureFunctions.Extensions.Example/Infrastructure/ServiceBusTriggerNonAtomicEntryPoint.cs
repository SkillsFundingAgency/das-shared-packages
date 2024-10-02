using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.AzureFunctions.Extensions.Example.Infrastructure;

internal class ServiceBusTriggerNonAtomicEntryPoint
{
    private readonly IEndpointInstance endpoint;

    public ServiceBusTriggerNonAtomicEntryPoint(IEndpointInstance endpoint)
    {
        this.endpoint = endpoint;
    }

    [FunctionName("ExtensionExampleEntryPoint")]
    public async Task Run(
        [ServiceBusTrigger(queueName: QueueNames.ExtensionExample, Connection = "AzureWebJobsServiceBus")] ServiceBusReceivedMessage message,
        ILogger logger,
        ExecutionContext context)
    {
        await endpoint.Send(message);
    }
}