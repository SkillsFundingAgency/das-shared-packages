using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.EmployerUrlHelper
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmployerUrlHelper(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Configure<EmployerUrlConfiguration>(configuration.GetSection("SFA.DAS.EmployerUrlHelper"))
                .AddSingleton<ILinkGenerator, LinkGenerator>();
        }
    }
}