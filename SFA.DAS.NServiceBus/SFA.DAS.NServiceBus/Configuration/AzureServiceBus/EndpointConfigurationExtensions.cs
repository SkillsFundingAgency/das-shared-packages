using System;
using NServiceBus;

namespace SFA.DAS.NServiceBus.Configuration.AzureServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config, string connectionString, Action<RoutingSettings> routing = null)
        {
#if NET6_0
            connectionString = connectionString.Replace("Endpoint=sb://", "");
            var transport = config.UseTransport<AzureServiceBusTransport>();
            transport.ConnectionString(connectionString);
            transport.Transactions(TransportTransactionMode.ReceiveOnly);
            transport.CustomTokenCredential(new Azure.Identity.DefaultAzureCredential());
            routing?.Invoke(transport.Routing());
#elif NETSTANDARD2_0
            var transport = config.UseTransport<AzureServiceBusTransport>();
            var ruleNameShortener = new RuleNameShortener();

            var tokenProvider = Microsoft.Azure.ServiceBus.Primitives.TokenProvider.CreateManagedIdentityTokenProvider();
            transport.CustomTokenProvider(tokenProvider);
            transport.ConnectionString(connectionString);
            transport.RuleNameShortener(ruleNameShortener.Shorten);
            transport.Transactions(TransportTransactionMode.ReceiveOnly);
#endif

            return config;
        }
    }
}