using Microsoft.Extensions.Hosting;

namespace SFA.DAS.NServiceBus
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseNServiceBusContainer(this IHostBuilder builder)
        {
            return builder.UseServiceProviderFactory(new NServiceBusServiceProviderFactory());
        }
    }
}