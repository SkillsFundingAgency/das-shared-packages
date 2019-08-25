using Microsoft.Extensions.Hosting;

namespace SFA.DAS.NServiceBus.DependencyResolution.Microsoft
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseNServiceBusContainer(this IHostBuilder builder)
        {
            return builder.UseServiceProviderFactory(new NServiceBusServiceProviderFactory());
        }
    }
}