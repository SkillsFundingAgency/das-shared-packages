using System;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

namespace SFA.DAS.NServiceBus.Configuration.AzureServiceBus;

public static class EndpointConfigurationExtensions
{
    public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config, IConfiguration appConfiguration,
        string connectionString, Action<RoutingSettings> routing = null)
    {
        var transport = config.UseTransport<AzureServiceBusTransport>();

        transport.CustomTokenCredential(appConfiguration["AzureWebJobsServiceBus:fullyQualifiedNamespace"], new DefaultAzureCredential());
        transport.ConnectionString(connectionString.FormatConnectionString());
        transport.Transactions(TransportTransactionMode.ReceiveOnly);
        transport.SubscriptionRuleNamingConvention(RuleNameShortener.Shorten);
        routing?.Invoke(transport.Routing());

        return config;
    }

    public static EndpointConfiguration UseEndpointWithExternallyManagedService(this EndpointConfiguration config, IServiceCollection services)
    {
        var endpointInstance = Endpoint.Start(config).GetAwaiter().GetResult();

        services.AddSingleton<IEndpointInstance>(endpointInstance);

        return config;
    }
}