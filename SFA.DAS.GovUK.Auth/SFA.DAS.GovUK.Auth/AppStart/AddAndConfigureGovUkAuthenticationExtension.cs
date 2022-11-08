using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    public static class AddAndConfigureGovUkAuthenticationExtension
    {
        public static void AddAndConfigureGovUkAuthentication(this IServiceCollection services,
            IConfiguration configuration, string authenticationCookieName, Type customClaims)
        {
            services.AddServiceRegistration(configuration, customClaims);
            if (!string.IsNullOrEmpty(configuration["NoAuthEmail"]))
            {
                services.AddEmployerStubAuthentication($"{authenticationCookieName}.stub");
            }
            else
            {
                services.ConfigureGovUkAuthentication(configuration, authenticationCookieName);
            }

        }
    }
}