using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseNServiceBusContainer(this IWebHostBuilder builder)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddSingleton<NServiceBusServiceProviderFactory>();
            });

        }
    }
}