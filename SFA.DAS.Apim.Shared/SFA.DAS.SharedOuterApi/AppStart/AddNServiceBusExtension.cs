using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.SharedOuterApi.Configuration;

namespace SFA.DAS.SharedOuterApi.AppStart
{
    public static class AddNServiceBusExtension
    {
        public static UpdateableServiceProvider StartNServiceBus(
            this UpdateableServiceProvider serviceProvider,
            IConfiguration configuration,
            string endpointName)
        {
            var config = configuration
                .GetSection("NServiceBusConfiguration")
                .Get<NServiceBusConfiguration>();

            var endpointConfiguration = new EndpointConfiguration(endpointName)
                .UseErrorQueue($"{endpointName}-errors")
                .UseInstallers()
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer();

            if (!string.IsNullOrEmpty(config.NServiceBusLicense))
            {
                endpointConfiguration.UseLicense(config.NServiceBusLicense);
            }

            endpointConfiguration.SendOnly();

            if (config.NServiceBusConnectionString.Equals("UseLearningEndpoint=true", StringComparison.CurrentCultureIgnoreCase))
            {
                endpointConfiguration.UseLearningTransport(s => s.AddRouting());

            }
            else
            {
                endpointConfiguration.UseAzureServiceBusTransport(config.NServiceBusConnectionString, s => s.AddRouting());
            }

            var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            serviceProvider.AddSingleton(p => endpoint)
                .AddSingleton<IMessageSession>(p => p.GetService<IEndpointInstance>())
                .AddHostedService<NServiceBusHostedService>();

            return serviceProvider;
        }
    }
}
