using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection
{
    public static class WebHostBuilderExtensions
    {
        public static WebApplicationBuilder UseNServiceBusContainer(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<NServiceBusServiceProviderFactory>();
            return builder;
        }
    }
}