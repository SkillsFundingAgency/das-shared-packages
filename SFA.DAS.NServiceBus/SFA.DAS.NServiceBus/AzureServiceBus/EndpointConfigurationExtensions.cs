using System;
using NServiceBus;
#if NET462
using NServiceBus.Transport.AzureServiceBus;
#endif

namespace SFA.DAS.NServiceBus.AzureServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseAzureServiceBusTransport(this EndpointConfiguration config, string connectionString, Action<RoutingSettings> routing = null)
        {
#if NETSTANDARD2_0
                var transport = config.UseTransport<AzureServiceBusTransport>();
                var ruleNameShortener = new RuleNameShortener();

                transport.ConnectionString(connectionString);
                transport.RuleNameShortener(ruleNameShortener.Shorten);
                transport.Transactions(TransportTransactionMode.ReceiveOnly);

                routing?.Invoke(transport.Routing());
#elif NET462
                var transport = config.UseTransport<AzureServiceBusTransport>();

                transport.BrokeredMessageBodyType(SupportedBrokeredMessageBodyTypes.Stream);
                transport.ConnectionString(connectionString);
                transport.Transactions(TransportTransactionMode.ReceiveOnly);
                transport.UseForwardingTopology();

                var messageReceivers = transport.MessageReceivers();

                messageReceivers.AutoRenewTimeout(TimeSpan.FromMinutes(10));

                var queues = transport.Queues();

                queues.ForwardDeadLetteredMessagesTo(q => q != "error" && q != "audit" && q != "deadletter", "deadletter");
                queues.LockDuration(TimeSpan.FromMinutes(1));

                var subscriptions = transport.Subscriptions();

                subscriptions.ForwardDeadLetteredMessagesTo("deadletters");

                var sanitization = transport.Sanitization();

                sanitization.UseStrategy<ValidateAndHashIfNeeded>();

                routing?.Invoke(transport.Routing());
#endif

            return config;
        }
    }
}