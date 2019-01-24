using System;
using NServiceBus;
using NServiceBus.Transport.AzureServiceBus;

namespace SFA.DAS.NServiceBus.AzureServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config, bool isDevelopment, Func<string> connectionStringBuilder, Action<RoutingSettings> routing)
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
                var ruleNameShortener = new RuleNameShortener();

                transport.ConnectionString(connectionStringBuilder);
                transport.RuleNameShortener(ruleNameShortener.Shorten);
                transport.Transactions(TransportTransactionMode.ReceiveOnly);

                routing(transport.Routing());
#elif NET462
                var transport = config.UseTransport<AzureServiceBusTransport>();

                transport.BrokeredMessageBodyType(SupportedBrokeredMessageBodyTypes.Stream);
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