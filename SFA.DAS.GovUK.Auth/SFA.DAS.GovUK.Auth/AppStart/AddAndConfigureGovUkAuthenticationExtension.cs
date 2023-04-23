using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.GovUK.Auth.Extensions;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    public static class AddAndConfigureGovUkAuthenticationExtension
    {
        public static void AddAndConfigureGovUkAuthentication(this IServiceCollection services,
            IConfiguration configuration, string authenticationCookieName, Type customClaims, string signedOutRedirectUrl = "", string localStubLoginPath = "")
        {
            bool.TryParse(configuration["StubAuth"],out var stubAuth);
            services.AddServiceRegistration(configuration, customClaims);
            if (stubAuth && configuration["ResourceEnvironmentName"].ToUpper() != "PRD")
            {
                services.AddEmployerStubAuthentication($"{authenticationCookieName}.stub",
                    signedOutRedirectUrl.GetSignedOutRedirectUrl(configuration["ResourceEnvironmentName"]),
                    RedirectExtension.GetStubSignInRedirectUrl(configuration["ResourceEnvironmentName"]),
                    localStubLoginPath);
            }
            else
            {
                services.ConfigureGovUkAuthentication(configuration, authenticationCookieName,signedOutRedirectUrl.GetSignedOutRedirectUrl(configuration["ResourceEnvironmentName"]));
            }

        }
    }
}