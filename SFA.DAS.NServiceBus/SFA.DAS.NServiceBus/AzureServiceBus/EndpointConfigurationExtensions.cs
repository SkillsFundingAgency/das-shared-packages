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
#if NETSTANDARD2_0
                var transport = config.UseTransport<AzureServiceBusTransport>();

                transport.ConnectionString(connectionStringBuilder);
                transport.Transactions(TransportTransactionMode.ReceiveOnly);

                routing(transport.Routing());
#elif NET462
                var transport = config.UseTransport<AzureServiceBusTransport>();

                transport.ConnectionString(connectionStringBuilder);
                transport.Transactions(TransportTransactionMode.ReceiveOnly);
                transport.UseForwardingTopology();

                var messageReceivers = transport.MessageReceivers();

                messageReceivers.AutoRenewTimeout(TimeSpan.FromMinutes(10));

                var queues = transport.Queues();

                queues.ForwardDeadLetteredMessagesTo(q => q != "errors" && q != "audits" && q != "deadletters", "deadletters");
                queues.LockDuration(TimeSpan.FromMinutes(1));

                var subscriptions = transport.Subscriptions();

                subscriptions.ForwardDeadLetteredMessagesTo("deadletters");

                var sanitization = transport.Sanitization();

                sanitization.UseStrategy<ValidateAndHashIfNeeded>();

                routing(transport.Routing());
#endif
            }

            return config;
        }
    }
}