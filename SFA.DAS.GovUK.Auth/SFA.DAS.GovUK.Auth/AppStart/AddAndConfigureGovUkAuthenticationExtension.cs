using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.GovUK.Auth.Extensions;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    public static class AddAndConfigureGovUkAuthenticationExtension
    {
        public static void AddAndConfigureGovUkAuthentication(this IServiceCollection services,
            IConfiguration configuration, string authenticationCookieName, Type customClaims, string signedOutRedirectUrl = "")
        {
            services.AddServiceRegistration(configuration, customClaims);
            if (!string.IsNullOrEmpty(configuration["NoAuthEmail"]))
            {
                services.AddEmployerStubAuthentication($"{authenticationCookieName}.stub",signedOutRedirectUrl.GetSignedOutRedirectUrl(configuration["ResourceEnvironmentName"]));
            }
            else
            {
                services.ConfigureGovUkAuthentication(configuration, authenticationCookieName,signedOutRedirectUrl.GetSignedOutRedirectUrl(configuration["ResourceEnvironmentName"]));
            }

        }
    }
}