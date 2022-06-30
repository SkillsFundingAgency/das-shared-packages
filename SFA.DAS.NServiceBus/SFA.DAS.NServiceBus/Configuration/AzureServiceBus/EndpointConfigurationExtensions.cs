using System;
using NServiceBus;
#if NET6_0
using Azure.Identity;
#endif
#if NETSTANDARD2_0
using Microsoft.Azure.ServiceBus.Primitives;
#elif NET462
using Microsoft.ServiceBus;
using NServiceBus.Transport.AzureServiceBus;
#endif

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
            transport.CustomTokenCredential(new DefaultAzureCredential());
            routing?.Invoke(transport.Routing());
#elif NETSTANDARD2_0
            var transport = config.UseTransport<AzureServiceBusTransport>();
            var ruleNameShortener = new RuleNameShortener();

            var tokenProvider = TokenProvider.CreateManagedIdentityTokenProvider();
            transport.CustomTokenProvider(tokenProvider);
            transport.ConnectionString(connectionString);
            transport.RuleNameShortener(ruleNameShortener.Shorten);
            transport.Transactions(TransportTransactionMode.ReceiveOnly);
#elif NET462
            var transport = config.UseTransport<AzureServiceBusTransport>();

                transport.BrokeredMessageBodyType(SupportedBrokeredMessageBodyTypes.Stream);
                var managers = transport.NamespaceManagers();
                managers.NamespaceManagerSettingsFactory(
                    factory: s =>
                    {
                        return new NamespaceManagerSettings {
                            TokenProvider = TokenProvider.CreateManagedServiceIdentityTokenProvider(ServiceAudience.ServiceBusAudience)
                        };
                    });
                transport.ConnectionString(connectionString);
                transport.Transactions(TransportTransactionMode.ReceiveOnly);
                transport.UseForwardingTopology();

                var messageReceivers = transport.MessageReceivers();

                messageReceivers.AutoRenewTimeout(TimeSpan.FromMinutes(10));

                var queues = transport.Queues();

                queues.ForwardDeadLetteredMessagesTo(q => q != "error" && q != "audit" && q != "deadletters", "deadletters");
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