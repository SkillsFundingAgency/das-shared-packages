using System;
using Azure.Identity;
using NServiceBus;

namespace SFA.DAS.NServiceBus.Configuration.AzureServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config,
            string connectionString, Action<RoutingSettings> routing = null)
        {
            var transport = config.UseTransport<AzureServiceBusTransport>();
            transport.CustomTokenCredential(new DefaultAzureCredential());
            transport.ConnectionString(connectionString.Replace("Endpoint=sb://", ""));
            transport.Transactions(TransportTransactionMode.ReceiveOnly);
            transport.SubscriptionRuleNamingConvention(RuleNameShortener.Shorten);
            routing?.Invoke(transport.Routing());

            return config;
        }
    }
}