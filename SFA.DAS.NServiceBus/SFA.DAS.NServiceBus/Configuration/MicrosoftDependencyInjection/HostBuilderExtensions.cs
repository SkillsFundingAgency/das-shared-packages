using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseNServiceBusContainer(this IHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                var endpointConfiguration = new EndpointConfiguration("YourEndpointName");

                var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

                services.AddSingleton<IEndpointInstance>(endpointInstance);
            });

            return builder;
        }

    }
}