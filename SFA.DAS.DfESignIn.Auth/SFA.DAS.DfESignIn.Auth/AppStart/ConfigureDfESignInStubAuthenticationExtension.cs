using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Web;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.DfESignIn.Auth.Constants;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    internal static class ConfigureDfESignInStubAuthenticationExtension
    {
        internal static void AddProviderStubAuthentication(
            this IServiceCollection services,
            string redirectUrl,
            string loginRedirect,
            string localRedirect,
            string cookieDomain)
        {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = localRedirect;
                    options.AccessDeniedPath = new PathString("/error/403");
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.Cookie.Name = StubAuthConstants.CookieName;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                    options.Cookie.IsEssential = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    if (!string.IsNullOrEmpty(cookieDomain))
                    {
                        options.Cookie.Domain = cookieDomain;
                    }
                    options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
                    options.SlidingExpiration = true;
                    options.LogoutPath = "/signed-out";
                    options.Events.OnSigningOut = c =>
                    {
                        c.Response.Cookies.Delete(StubAuthConstants.CookieName);
                        c.Response.Redirect(redirectUrl);
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

            services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<ICustomClaims>((options, customClaims) =>
                {
                    options.Events.OnValidatePrincipal = async (ctx) =>
                    {
                        var claims = new List<Claim>();
                        claims.AddRange(ctx.Principal.Claims);

                        if (customClaims != null)
                        {
                            var additionalClaims = customClaims.GetClaims(new TokenValidatedContext(ctx.HttpContext, new AuthenticationScheme(CookieAuthenticationDefaults.AuthenticationScheme, "Cookie", typeof(ProviderStubAuthHandler)), new OpenIdConnectOptions(), ctx.Principal, new AuthenticationProperties()));
                            foreach (var additionalClaim in additionalClaims)
                            {
                                if (claims.FirstOrDefault(c => c.Type == additionalClaim.Type) == null)
                                    claims.Add(additionalClaim);
                            }

                        }
                        ctx.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
                        await ctx.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, ctx.Principal);
                    };
                });
        }
    }
}