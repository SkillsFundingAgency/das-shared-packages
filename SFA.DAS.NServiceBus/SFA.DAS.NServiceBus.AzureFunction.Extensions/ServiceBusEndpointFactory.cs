using Azure.Identity;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using SFA.DAS.NServiceBus.Extensions;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions;

public static class ServiceBusEndpointFactory
{
    public static ServiceBusTriggeredEndpointConfiguration CreateSingleQueueConfiguration(string endpointAndQueueName, IConfiguration appConfiguration, bool useManagedIdentity)
    {
        var configuration = new ServiceBusTriggeredEndpointConfiguration(
            endpointName: endpointAndQueueName,
            configuration: appConfiguration);

        configuration.AdvancedConfiguration.SendFailedMessagesTo($"{endpointAndQueueName}-error");
        configuration.AdvancedConfiguration.Pipeline.Register(new LogIncomingBehaviour(), nameof(LogIncomingBehaviour));
        configuration.AdvancedConfiguration.Pipeline.Register(new LogOutgoingBehaviour(), nameof(LogOutgoingBehaviour));

        if (useManagedIdentity)
        {
            configuration.Transport.ConnectionString(appConfiguration.NServiceBusFullNamespace());
            configuration.Transport.CustomTokenCredential(new DefaultAzureCredential());
            configuration.AdvancedConfiguration.License(appConfiguration.NServiceBusLicense());
        }

        configuration.Transport.SubscriptionRuleNamingConvention(AzureRuleNameShortener.Shorten);

        var persistence = configuration.AdvancedConfiguration.UsePersistence<AzureTablePersistence>();
        persistence.ConnectionString(appConfiguration.GetConnectionStringOrSetting("AzureWebJobsStorage"));

        return configuration;
    }
}