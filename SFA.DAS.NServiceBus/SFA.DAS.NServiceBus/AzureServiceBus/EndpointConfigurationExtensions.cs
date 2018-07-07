using System;
using NServiceBus;

namespace SFA.DAS.NServiceBus.AzureServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration SetupAzureServiceBusTransport(this EndpointConfiguration config, bool isDevelopment, Func<string> connectionStringBuilder, Action<RoutingSettings> routing)
        {
            if (isDevelopment)
            {
                var transport = config.UseTransport<LearningTransport>();

                transport.Transactions(TransportTransactionMode.ReceiveOnly);
                
                routing(transport.Routing());
            }
            else
            {
                var transport = config.UseTransport<AzureServiceBusTransport>();

                transport.ConnectionString(connectionStringBuilder);
                transport.Transactions(TransportTransactionMode.ReceiveOnly);
                transport.UseForwardingTopology();
                
                routing(transport.Routing());
            }

            return config;
        }
    }
}