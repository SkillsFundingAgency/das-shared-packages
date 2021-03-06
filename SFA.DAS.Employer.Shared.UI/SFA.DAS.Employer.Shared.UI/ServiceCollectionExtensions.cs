using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Employer.Shared.UI.Configuration;
using SFA.DAS.EmployerUrlHelper;
using SFA.DAS.EmployerUrlHelper.DependencyResolution;

namespace SFA.DAS.Employer.Shared.UI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMaMenuConfiguration(this IServiceCollection services, IConfiguration configuration, string logoutRouteName, string identityClientId)
        {
            ValidateArguments(logoutRouteName, identityClientId);

            services.AddEmployerUrlHelper(configuration);
            services.AddSingleton<UrlBuilder>();

            var linkGenerator = services.BuildServiceProvider().GetService<ILinkGenerator>();

            // TODO: Validate configuration values?
            services.Configure<MaPageConfiguration>(configuration.GetSection("SFA.DAS.Employer.Shared.UI:MaPageConfiguration"));
            services.PostConfigure<MaPageConfiguration>(options =>
            {
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