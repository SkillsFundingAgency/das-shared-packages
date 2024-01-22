using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.GovUK.Auth.Extensions;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    public static class AddAndConfigureGovUkAuthenticationExtension
    {
        public static void AddAndConfigureGovUkAuthentication(this IServiceCollection services,
            IConfiguration configuration, Type customClaims, string signedOutRedirectUrl = "", string localStubLoginPath = "", string cookieDomain = "", string loginRedirect = "")
        {
            bool.TryParse(configuration["StubAuth"],out var stubAuth);
            services.AddServiceRegistration(configuration, customClaims);
            if (stubAuth && configuration["ResourceEnvironmentName"].ToUpper() != "PRD")
            {
                services.AddEmployerStubAuthentication(signedOutRedirectUrl.GetSignedOutRedirectUrl(configuration["ResourceEnvironmentName"]),
                    loginRedirect.GetStubSignInRedirectUrl(configuration["ResourceEnvironmentName"]),
                    localStubLoginPath,
                    cookieDomain.GetEnvironmentAndDomain(configuration["ResourceEnvironmentName"]));
            }
            else
            {
                services.ConfigureGovUkAuthentication(configuration , signedOutRedirectUrl.GetSignedOutRedirectUrl(configuration["ResourceEnvironmentName"]),cookieDomain.GetEnvironmentAndDomain(configuration["ResourceEnvironmentName"]));
            }

        }
    }
}