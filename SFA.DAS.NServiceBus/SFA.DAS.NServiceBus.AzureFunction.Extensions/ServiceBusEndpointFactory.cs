using Microsoft.Extensions.Configuration;
using NServiceBus;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions;

public static class ServiceBusEndpointFactory
{
    public static ServiceBusTriggeredEndpointConfiguration CreateSingleQueueConfiguration(string endpointAndQueueName, IConfiguration appConfiguration)
    {

        var configuration = new ServiceBusTriggeredEndpointConfiguration(
            endpointName: endpointAndQueueName,
            configuration: appConfiguration);

        configuration.AdvancedConfiguration.SendFailedMessagesTo($"{endpointAndQueueName}-error");
        configuration.AdvancedConfiguration.Pipeline.Register(new LogIncomingBehaviour(), nameof(LogIncomingBehaviour));
        configuration.AdvancedConfiguration.Pipeline.Register(new LogOutgoingBehaviour(), nameof(LogOutgoingBehaviour));

        configuration.Transport.SubscriptionRuleNamingConvention(AzureRuleNameShortener.Shorten);

        return configuration;
    }
}