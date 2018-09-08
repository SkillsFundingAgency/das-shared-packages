#if NETCOREAPP2_0
using Microsoft.AspNetCore.Builder;

namespace SFA.DAS.NServiceBus.Mvc
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseNServiceBusOutbox(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UnitOfWorkManagerMiddleware>();
        }
    }
}
#endif