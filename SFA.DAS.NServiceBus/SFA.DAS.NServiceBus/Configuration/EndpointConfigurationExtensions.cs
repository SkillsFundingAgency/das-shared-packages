using System;
using System.Text.RegularExpressions;
using NServiceBus;

namespace SFA.DAS.NServiceBus.Configuration
{
    public static class EndpointConfigurationExtensions
    {
        [Obsolete("This will be removed in an upcoming release, please use the override requiring the errorQueue parameter", false)]
        public static EndpointConfiguration UseErrorQueue(this EndpointConfiguration config)
        {
            return UseErrorQueue(config, "errors");
        }

        public static EndpointConfiguration UseErrorQueue(this EndpointConfiguration config, string errorQueue)
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
            
#pragma warning disable 618
            conventions.DefiningCommandsAs(t => Regex.IsMatch(t.Name, @"Command(V\d+)?$") || typeof(Command).IsAssignableFrom(t));
            conventions.DefiningEventsAs(t => Regex.IsMatch(t.Name, @"Event(V\d+)?$") || typeof(Event).IsAssignableFrom(t));
#pragma warning restore 618

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
    }
}