using System;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;

namespace SFA.DAS.NServiceBus.Configuration.AzureServiceBus;

public static class EndpointConfigurationExtensions
{
    public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config,
        string connectionString, Action<RoutingSettings> routing = null)
    {
        var transport = config.UseTransport<AzureServiceBusTransport>();
        transport.CustomTokenCredential(new DefaultAzureCredential());
        transport.ConnectionString(connectionString.FormatConnectionString());
        transport.Transactions(TransportTransactionMode.ReceiveOnly);
        transport.SubscriptionRuleNamingConvention(RuleNameShortener.Shorten);
        routing?.Invoke(transport.Routing());

        return config;
    }

    public static EndpointConfiguration UseEndpointWithExternallyManagedService(this EndpointConfiguration config, IServiceCollection services)
    {
        var endpointWithExternallyManagedServiceProvider = EndpointWithExternallyManagedServiceProvider.Create(config, services);
        endpointWithExternallyManagedServiceProvider.Start(new UpdateableServiceProvider(services));
        services.AddSingleton(_ => endpointWithExternallyManagedServiceProvider.MessageSession.Value);

        return config;
    }
}