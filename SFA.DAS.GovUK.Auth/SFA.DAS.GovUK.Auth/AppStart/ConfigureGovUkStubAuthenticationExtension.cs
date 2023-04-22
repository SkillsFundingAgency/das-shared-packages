using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Extensions;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    internal static class ConfigureGovUkStubAuthenticationExtension
    {

        public static void AddEmployerStubAuthentication(this IServiceCollection services,
            string authenticationCookieName, string redirectUrl, string loginRedirect)
        {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                //.AddScheme<AuthenticationSchemeOptions, EmployerStubAuthHandler>(authenticationCookieName, _ => { })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/home/AccountDetails";
                    options.AccessDeniedPath = new PathString("/error/403");
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.Cookie.Name = authenticationCookieName;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
                    options.LogoutPath = "/home/signed-out";
                    options.Events.OnSigningOut = c =>
                    {
                        c.Response.Cookies.Delete(authenticationCookieName);
                        c.Response.Cookies.Delete(GovUkConstants.StubAuthCookieName);
                        c.Response.Redirect(redirectUrl);
                        return Task.CompletedTask;
                    };
                    if (!string.IsNullOrEmpty(loginRedirect))
                    {
                        options.Events.OnRedirectToLogin = c =>
                        {
                            var redirectQuery = new Uri(c.RedirectUri).Query;
                            c.Response.Redirect(loginRedirect + redirectQuery);
                            return Task.CompletedTask;
                        };    
                    }
                    
                });

            //services.AddAuthentication(authenticationCookieName).AddAuthenticationCookie(authenticationCookieName);
        }

    }
}