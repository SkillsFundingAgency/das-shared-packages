using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    public static class AddAndConfigureDfESignInAuthenticationExtension
    {
        public static void AddAndConfigureDfESignInAuthentication(
            this IServiceCollection services,
            IConfiguration configuration,
            string authenticationCookieName,
            Type customServiceRole,
            string clientName,
            string signedOutCallbackPath = "/signed-out")
        {
            services.AddServiceRegistration(configuration, customServiceRole, clientName);
            if (!string.IsNullOrEmpty(configuration["NoAuthEmail"]))
            {
                services.AddProviderStubAuthentication($"{authenticationCookieName}.stub");
            }
            else
            {
                services.ConfigureDfESignInAuthentication(configuration, authenticationCookieName, clientName, signedOutCallbackPath);
            }
        }
    }
}