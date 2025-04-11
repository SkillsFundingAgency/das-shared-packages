using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.Enums;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    internal static class ConfigureDfESignInStubAuthenticationExtension
    {
        internal static void AddProviderStubAuthentication(
            this IServiceCollection services,
            string authenticationCookieName,
            string signedOutCallbackPath, 
            string resourceEnvironmentName,
            ClientName clientName)
        {
            services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, ProviderStubAuthHandler>(authenticationCookieName, _ => { })
                .AddCookie(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Events.OnSigningOut = c =>
                    {
                        c.Response.Cookies.Delete(authenticationCookieName);
                        c.Response.Redirect("/");
                        return Task.CompletedTask;
                    };
                });

            services.AddAuthentication(authenticationCookieName).AddAuthenticationCookie(authenticationCookieName, signedOutCallbackPath, resourceEnvironmentName, clientName);
        }
    }
}