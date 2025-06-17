using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.GovUK.Auth.Configuration;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    internal static class ConfigureGovUkStubAuthenticationExtension
    {

        public static void ConfigureStubAuthentication(this IServiceCollection services, string signedOutRedirectUrl, string loginRedirect, string localStubLoginPath, string cookieDomain)
        {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = localStubLoginPath;
                    options.AccessDeniedPath = new PathString("/error/403");
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.Cookie.Name = GovUkConstants.StubAuthCookieName;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.Cookie.IsEssential = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    if (!string.IsNullOrEmpty(cookieDomain))
                    {
                        options.Cookie.Domain = cookieDomain;
                    }
                    options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
                    options.LogoutPath = "/home/signed-out";

                    options.Events.OnSigningOut = c =>
                    {
                        c.Response.Cookies.Delete(GovUkConstants.StubAuthCookieName);
                        c.Response.Redirect(signedOutRedirectUrl);
                        return Task.CompletedTask;
                    };
                    
                    if (!string.IsNullOrEmpty(loginRedirect))
                    {
                        options.Events.OnRedirectToLogin = c =>
                        {
                            var redirectUri = new Uri(c.RedirectUri);

                            var redirectQuery = HttpUtility.UrlEncode(
                                $"{redirectUri.Scheme}://{redirectUri.Authority}{HttpUtility.UrlDecode(redirectUri.Query.Replace("?ReturnUrl=", ""))}");
                            c.Response.Redirect(loginRedirect + "?ReturnUrl=" + redirectQuery);
                            return Task.CompletedTask;
                        };
                    }
                });
        }
    }
}