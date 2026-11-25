using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Enums;
using System;
using System.Threading.Tasks;

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

        internal static void ConfigureDfESignInStubAuthentication(
            this IServiceCollection services,
            DfEOidcConfiguration configuration,
            string authenticationCookieName,
            string signedOutCallbackPath)
        {
            services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddAuthenticationCookie(
                    $"{authenticationCookieName}.stub",
                    signedOutCallbackPath,
                    configuration.CookieDomain);

            services
                .AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<ITicketStore, DfEOidcConfiguration>(
                    (options, ticketStore, config) =>
                    {
                        options.LoginPath = new PathString("/stub/sign-in-stub");

                        options.ExpireTimeSpan = TimeSpan.FromMinutes(
                            config.LoginSlidingExpiryTimeOutInMinutes);

                        options.SlidingExpiration = true;
                        options.SessionStore = ticketStore;

                        options.Events.OnSigningOut = context =>
                        {
                            context.Response.Redirect(config.SignedOutRedirectUrl);

                            return Task.CompletedTask;
                        };
                    });
        }
    }
}