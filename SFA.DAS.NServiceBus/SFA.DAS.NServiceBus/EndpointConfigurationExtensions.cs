using System;
using System.Text.RegularExpressions;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;

namespace SFA.DAS.NServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseErrorQueue(this EndpointConfiguration config, string errorQueue = "error")
        {
            config.SendFailedMessagesTo(errorQueue);

            return config;
        }

        public static EndpointConfiguration UseHeartbeat(this EndpointConfiguration config)
        {
            config.SendHeartbeatTo("heartbeat");

            return config;
        }

        public static EndpointConfiguration UseInstallers(this EndpointConfiguration config)
        {
            config.EnableInstallers();

            return config;
        }

        public static EndpointConfiguration UseLearningTransport(this EndpointConfiguration config, Action<RoutingSettings> routing = null)
        {
            var transport = config.UseTransport<LearningTransport>();
                
            transport.Transactions(TransportTransactionMode.ReceiveOnly);

            routing?.Invoke(transport.Routing());

            return config;
        }
        
        public static EndpointConfiguration UseLicense(this EndpointConfiguration config, string licenseText)
        {
            config.License(licenseText);

            return config;
        }
        
        public static EndpointConfiguration UseMessageConventions(this EndpointConfiguration config)
        {
            var conventions = config.Conventions();
            
            conventions.DefiningCommandsAs(t => Regex.IsMatch(t.Name, @"Command(V\d+)?$"));
            conventions.DefiningEventsAs(t => Regex.IsMatch(t.Name, @"Event(V\d+)?$"));

            return config;
        }

        public static EndpointConfiguration UseMetrics(this EndpointConfiguration config)
        {
            var metrics = config.EnableMetrics();

            metrics.SendMetricDataToServiceControl("particular.monitoring", TimeSpan.FromSeconds(10));

            return config;
        }

        public static EndpointConfiguration UsePurgeOnStartup(this EndpointConfiguration config)
        {
            config.PurgeOnStartup(true);

            return config;
        }

        public static EndpointConfiguration UseSendOnly(this EndpointConfiguration config)
        {
            config.SendOnly();

            return config;
        }

        public static EndpointConfiguration UseServicesBuilder(this EndpointConfiguration config, UpdateableServiceProvider serviceProvider)
        {
            config.UseContainer<ServicesBuilder>(c => c.ServiceProviderFactory(s => serviceProvider));

            return config;
        }
    }
}