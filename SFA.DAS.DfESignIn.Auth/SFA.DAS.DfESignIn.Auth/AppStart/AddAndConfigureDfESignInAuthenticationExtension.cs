using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    public static class AddAndConfigureDfESignInAuthenticationExtension
    {
        public static void AddAndConfigureDfESignInAuthentication(this IServiceCollection services, IConfiguration configuration, string authenticationCookieName, Type customServiceRole)
        {
            services.AddServiceRegistration(configuration, customServiceRole);
            if (!string.IsNullOrEmpty(configuration["NoAuthEmail"]))
            {
                services.AddProviderStubAuthentication($"{authenticationCookieName}.stub");
            }
            else
            {
                services.ConfigureDfESignInAuthentication(configuration, authenticationCookieName);
            }
        }
    }
}