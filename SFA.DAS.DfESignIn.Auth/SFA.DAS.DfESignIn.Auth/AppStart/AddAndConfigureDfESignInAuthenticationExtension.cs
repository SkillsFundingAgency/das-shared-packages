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
            Type customClaims,
            ClientName clientName,
            string signedOutCallbackPath = "/signed-out",
            string signedOutRedirectUrl = "",
            string cookieDomain = "",
            string localStubLoginPath = "",
            string loginRedirect = "")
        {
            var env = configuration["ResourceEnvironmentName"];

            bool.TryParse(configuration["StubAuth"], out var stubAuth);

            services.AddServiceRegistration(configuration, customClaims, clientName);
            if(stubAuth && env.ToUpper() != "PRD")
            {
                services.AddProviderStubAuthentication(signedOutRedirectUrl.GetSignedOutRedirectUrl(env, clientName),
                    loginRedirect.GetStubSignInRedirectUrl(env), 
                    localStubLoginPath,
                    cookieDomain.GetEnvironmentAndDomain(env));
            }
            else
            {
                services.ConfigureDfESignInAuthentication(configuration, authenticationCookieName, clientName, signedOutCallbackPath, signedOutRedirectUrl.GetSignedOutRedirectUrl(env, clientName));
            }
        }
    }
}