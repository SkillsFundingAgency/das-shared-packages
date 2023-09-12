using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Extensions;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    public static class AddAndConfigureDfESignInAuthenticationExtension
    {
        public static void AddAndConfigureDfESignInAuthentication(
            this IServiceCollection services,
            IConfiguration configuration,
            string authenticationCookieName,
            Type customServiceRole,
            ClientName clientName,
            string signedOutCallbackPath = "/signed-out",
            string redirectUrl = "")
        {
            services.AddServiceRegistration(configuration, customServiceRole, clientName);
            if (!string.IsNullOrEmpty(configuration["NoAuthEmail"]))
            {
                services.AddProviderStubAuthentication($"{authenticationCookieName}.stub", signedOutCallbackPath,configuration["ResourceEnvironmentName"], clientName);
            }
            else
            {
                services.ConfigureDfESignInAuthentication(configuration, authenticationCookieName, clientName, signedOutCallbackPath, redirectUrl.GetSignedOutRedirectUrl(configuration["ResourceEnvironmentName"], clientName));
            }
        }
    }
}