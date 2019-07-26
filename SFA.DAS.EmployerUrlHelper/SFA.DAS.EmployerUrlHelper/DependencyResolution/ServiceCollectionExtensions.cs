#if NETSTANDARD2_0
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.EmployerUrlHelper.Configuration;

namespace SFA.DAS.EmployerUrlHelper.DependencyResolution
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmployerUrlHelper(this IServiceCollection services)
        {
            return services.AddAutoConfiguration()
                .AddSingleton(p => p.GetRequiredService<IAutoConfigurationService>().Get<EmployerUrlHelperConfiguration>())
                .AddSingleton<ILinkGenerator, LinkGenerator>();
        }

        public static IServiceCollection AddEmployerUrlHelper(this IServiceCollection services, IConfiguration configuration)
        {
            var helperConfig = configuration.GetSection("SFA.DAS.EmployerUrlHelper").Get<EmployerUrlHelperConfiguration>();

            return services
                .AddSingleton(c => helperConfig)
                .AddSingleton<ILinkGenerator, LinkGenerator>();
        }
    }
}
#endif