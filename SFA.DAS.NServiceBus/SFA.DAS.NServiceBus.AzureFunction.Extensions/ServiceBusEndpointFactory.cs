using Azure.Identity;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using SFA.DAS.NServiceBus.Extensions;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions;

public static class ServiceBusEndpointFactory
{
    public static EndpointConfiguration CreateSingleQueueConfiguration(string endpointAndQueueName, IConfiguration appConfiguration, bool useManagedIdentity)
    {
        var configuration = new EndpointConfiguration("YourEndpointName");

        var transport = configuration.UseTransport<AzureServiceBusTransport>();

        // TODO
        //configuration.Ad.SendFailedMessagesTo($"{endpointAndQueueName}-error");
        //configuration.AdvancedConfiguration.Pipeline.Register(new LogIncomingBehaviour(), nameof(LogIncomingBehaviour));
        //configuration.AdvancedConfiguration.Pipeline.Register(new LogOutgoingBehaviour(), nameof(LogOutgoingBehaviour));

        if (useManagedIdentity)
        {
            transport.ConnectionString(appConfiguration.NServiceBusFullNamespace());
            transport.CustomTokenCredential(appConfiguration.NServiceBusFullNamespace(), new DefaultAzureCredential());
            // LICENSE?
        }

        transport.SubscriptionRuleNamingConvention(AzureRuleNameShortener.Shorten);

        var persistence = configuration.UsePersistence<AzureTablePersistence>();
        persistence.ConnectionString(appConfiguration.GetConnectionStringOrSetting("AzureWebJobsStorage"));

        return configuration;
    }
}