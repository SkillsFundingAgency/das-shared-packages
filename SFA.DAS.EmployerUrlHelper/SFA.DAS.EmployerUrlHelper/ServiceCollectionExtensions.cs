using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.EmployerUrlHelper
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmployerUrlHelper(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddSingleton(p =>
                {
                    var configurationSection = configuration.GetSection("SFA.DAS.EmployerUrlHelper");
                    var employerUrlConfiguration = configurationSection.Get<EmployerUrlConfiguration>();
                    
                    return employerUrlConfiguration;
                })
                .AddSingleton<ILinkGenerator, LinkGenerator>();
        }
    }
}