using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.GovUK.Auth.Extensions;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    public static class AddAndConfigureGovUkAuthenticationExtension
    {
        public static void AddAndConfigureGovUkAuthentication(this IServiceCollection services,
            IConfiguration configuration, AuthRedirects authRedirects, Type customClaims = null, Type employerAccountService = null)
        {
            bool.TryParse(configuration["StubAuth"],out var stubAuth);
            services.AddServiceRegistration(configuration, customClaims, employerAccountService);
            if (stubAuth && configuration["ResourceEnvironmentName"]!.ToUpper() != "PRD")
            {
                services.AddEmployerStubAuthentication(authRedirects.SignedOutRedirectUrl.GetSignedOutRedirectUrl(configuration["ResourceEnvironmentName"]),
                    authRedirects.LoginRedirect.GetStubSignInRedirectUrl(configuration["ResourceEnvironmentName"]),
                    authRedirects.LocalStubLoginPath,
                    authRedirects.CookieDomain.GetEnvironmentAndDomain(configuration["ResourceEnvironmentName"]));
            }
            else
            {
                services.ConfigureGovUkAuthentication(configuration , authRedirects.SignedOutRedirectUrl.GetSignedOutRedirectUrl(configuration["ResourceEnvironmentName"]), authRedirects.CookieDomain.GetEnvironmentAndDomain(configuration["ResourceEnvironmentName"]));
            }
        }
    }
}