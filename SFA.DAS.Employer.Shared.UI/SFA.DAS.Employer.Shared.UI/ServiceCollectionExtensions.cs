using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Employer.Shared.UI.Configuration;

namespace SFA.DAS.Employer.Shared.UI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMaMenuConfiguration(this IServiceCollection services, IConfiguration configuration, string logoutRouteName, string identityClientId)
        {
            ValidateArguments(logoutRouteName, identityClientId);
            
            // TODO: Validate configuration values?
            services.Configure<MaMenuConfiguration>(configuration.GetSection("SFA.DAS.Employer.Menu:MaPageConfiguration"));
            services.PostConfigure<MaMenuConfiguration>(options =>
            {
                options.ClientId = identityClientId;
                options.LocalLogoutRouteName = logoutRouteName;
            });
        }

        private static void ValidateArguments(string logoutRouteName, string identityClientId)
        {
            if (string.IsNullOrWhiteSpace(logoutRouteName))
            {
                throw new ArgumentException("Needs a valid value", nameof(logoutRouteName));
            }

            if (string.IsNullOrWhiteSpace(identityClientId))
            {
                throw new ArgumentException("Needs a valid value", nameof(identityClientId));
            }
        }
    }
}