using System;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
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

        public static void AddEmployerStubAuthentication(this IServiceCollection services, string redirectUrl, string loginRedirect, string localRedirect)
        {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = localRedirect;
                    options.AccessDeniedPath = new PathString("/error/403");
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.Cookie.Name = GovUkConstants.StubAuthCookieName;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
                    options.LogoutPath = "/home/signed-out";
                    options.Events.OnSigningOut = c =>
                    {
                        c.Response.Cookies.Delete(GovUkConstants.StubAuthCookieName);
                        c.Response.Redirect(redirectUrl);
                        return Task.CompletedTask;
                    };
                    if (!string.IsNullOrEmpty(loginRedirect))
                    {
                        options.Events.OnRedirectToLogin = c =>
                        {
                            var redirectUri = new Uri(c.RedirectUri);

                            var redirectQuery = HttpUtility.UrlEncode(
                                $"{redirectUri.Scheme}//{redirectUri.Authority}{HttpUtility.UrlDecode(redirectUri.Query.Replace("?ReturnUrl=", ""))}");
                            c.Response.Redirect(loginRedirect + "?ReturnUrl=" + redirectQuery);
                            return Task.CompletedTask;
                        };    
                    }
                    
                });

        }

    }
}