using System;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.NServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration UseErrorQueue(this EndpointConfiguration config)
        {
            config.SendFailedMessagesTo("errors");

            return config;
        }

        public static EndpointConfiguration UseHeartbeat(this EndpointConfiguration config)
        {
            config.SendHeartbeatTo("heartbeats");

            return config;
        }

        public static EndpointConfiguration UseInstallers(this EndpointConfiguration config)
        {
            config.EnableInstallers();

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
            conventions.DefiningCommandsAs(t => t.Name.EndsWith("Command") || t == typeof(Command));
            conventions.DefiningEventsAs(t => t.Name.EndsWith("Event") || t == typeof(Event));
#pragma warning restore 618

            return config;
        }

        public static EndpointConfiguration UseMetrics(this EndpointConfiguration config)
        {
            var metrics = config.EnableMetrics();

            metrics.SendMetricDataToServiceControl("particular.monitoring", TimeSpan.FromSeconds(10));

            return config;
        }

        public static EndpointConfiguration UseOutbox(this EndpointConfiguration config)
        {
            config.EnableOutbox();

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

        public static EndpointConfiguration UseServicesBuilder(this EndpointConfiguration config, IServiceCollection services)
        {
            services.AddTransient<IProcessClientOutboxMessagesJob, ProcessClientOutboxMessagesJob>();
            config.UseContainer<ServicesBuilder>(c => c.ExistingServices(services));

            return config;
        }
    }
}