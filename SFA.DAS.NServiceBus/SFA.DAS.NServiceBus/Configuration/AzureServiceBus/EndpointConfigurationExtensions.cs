using System;
using NServiceBus;

namespace SFA.DAS.NServiceBus.Configuration.AzureServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config,
            string connectionString, Action<RoutingSettings> routing = null)
        {
            var transport = config.UseTransport<AzureServiceBusTransport>();
#if NET6_0
            connectionString = connectionString.Replace("Endpoint=sb://", "");
            transport.CustomTokenCredential(new Azure.Identity.DefaultAzureCredential());
#else
            transport.CustomTokenProvider(Microsoft.Azure.ServiceBus.Primitives.TokenProvider
                .CreateManagedIdentityTokenProvider());
            transport.RuleNameShortener(new RuleNameShortener().Shorten);
#endif
            transport.ConnectionString(connectionString);
            transport.Transactions(TransportTransactionMode.ReceiveOnly);
            routing?.Invoke(transport.Routing());

            return config;
        }
    }
}