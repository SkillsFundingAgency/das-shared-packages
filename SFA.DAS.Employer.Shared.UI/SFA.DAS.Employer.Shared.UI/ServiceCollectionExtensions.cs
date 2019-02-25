using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Employer.Shared.UI.Configuration;

namespace SFA.DAS.Employer.Shared.UI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMaMenuConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MaMenuConfiguration>(configuration.GetSection("MaPageConfiguration"));
            services.PostConfigure<MaMenuConfiguration>(options =>
            {
                options.ClientId = configuration.GetValue<string>(options.IdentityClientIdConfigKey);
            });
        }
    }
}