using Azure.Identity;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using SFA.DAS.NServiceBus.Extensions;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions;

public class NServiceBusResourceManager
{
    private readonly Logger<ServiceBusAdministrationClient>? _logger;
    private readonly ServiceBusAdministrationClient _administrationClient;
    public NServiceBusResourceManager(IConfiguration configuration, bool useManagedIdentity, Logger<ServiceBusAdministrationClient>? logger = null)
    {
        _logger = logger;
        _administrationClient = useManagedIdentity ? new ServiceBusAdministrationClient(configuration.NServiceBusFullNamespace(), new DefaultAzureCredential()) : new ServiceBusAdministrationClient(configuration.NServiceBusSASConnectionString());
    }

    public async Task CreateWorkAndErrorQueues(string queueName)
    {
        await CreateQueue(queueName);
        await CreateQueue($"{queueName}-error");
    }

    private async Task CreateQueue(string queueName)
    {
        if (await _administrationClient.QueueExistsAsync(queueName)) return;

        _logger?.LogInformation("Creating queue: `{queueName}`", queueName);
        await _administrationClient.CreateQueueAsync(queueName);
    }

    public async Task SubscribeToTopicForQueue(Assembly myAssembly, string queueName, string topicName = "bundle-1")
    {
        var attribute = myAssembly.GetTypes()
            .SelectMany(t => t.GetMethods())
            .Where(m => m.GetCustomAttribute<FunctionNameAttribute>(false) != null)
            .SelectMany(m => m.GetParameters())
            .SelectMany(p => p.GetCustomAttributes<ServiceBusTriggerAttribute>(false))
            .FirstOrDefault();
        if (attribute == null)
            throw new NotSupportedException("No FunctionName or ServiceBusTrigger endpoint was found");

        await CreateSubscription(topicName, queueName);
    }

    private async Task CreateSubscription(string topicName, string queueName)
    {
        if (await _administrationClient.SubscriptionExistsAsync(topicName, queueName)) return;

        _logger?.LogInformation($"Creating subscription to: `{queueName}`", queueName);

        var createSubscriptionOptions = new CreateSubscriptionOptions(topicName, queueName) {
            ForwardTo = queueName,
            UserMetadata = $"Subscribed to {queueName}"
        };
        var createRuleOptions = new CreateRuleOptions() {
            Filter = new FalseRuleFilter()
        };

        await _administrationClient.CreateSubscriptionAsync(createSubscriptionOptions, createRuleOptions);
    }
}
