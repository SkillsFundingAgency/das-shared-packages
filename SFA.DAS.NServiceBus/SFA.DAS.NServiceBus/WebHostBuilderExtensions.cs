using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus.ObjectBuilder.MSDependencyInjection;

namespace SFA.DAS.NServiceBus
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseNServiceBusContainer(this IWebHostBuilder builder)
        {
            return builder.ConfigureServices(s => s.AddSingleton<IServiceProviderFactory<UpdateableServiceProvider>>(new NServiceBusServiceProviderFactory()));
        }
    }
}