using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.GovUK.Auth.Configuration;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    [ExcludeFromCodeCoverage]
    internal static class ConfigureGovUkStubAuthenticationExtension
    {
        public static void ConfigureStubAuthentication(this IServiceCollection services, string signedOutRedirectUrl, string loginRedirect, string localStubLoginPath, string cookieDomain)
        {
            services.AddScoped(sp =>
            {
                var config = sp.GetRequiredService<IOptions<GovUkOidcConfiguration>>();
                var ticketStore = sp.GetRequiredService<ITicketStore>();

                return new StubCookieAuthenticationEvents(
                    config,
                    ticketStore,
                    signedOutRedirectUrl,
                    loginRedirect);
            });

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = localStubLoginPath;
                    options.AccessDeniedPath = new PathString("/error/403");
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.Cookie.Name = GovUkConstants.StubAuthCookieName;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.Cookie.IsEssential = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
                    options.LogoutPath = "/home/signed-out";

                    if (!string.IsNullOrEmpty(cookieDomain))
                        options.Cookie.Domain = cookieDomain;

                    options.EventsType = typeof(StubCookieAuthenticationEvents);
                });

            services
                .AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<GovUkOidcConfiguration, ITicketStore>((options, config, ticketStore) =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(config.LoginSlidingExpiryTimeOutInMinutes);
                    options.SlidingExpiration = true;
                    options.SessionStore = ticketStore;
                });
        }
    }
}