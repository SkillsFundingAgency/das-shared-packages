#if NETCOREAPP
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AutoConfiguration.DependencyResolution;

namespace SFA.DAS.EmployerUrlHelper.DependencyResolution
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmployerUrlHelper(this IServiceCollection services)
        {
            return services.AddAutoConfiguration()
                .AddSingleton<ILinkGenerator, LinkGenerator>();
        }
    }
}
#endif