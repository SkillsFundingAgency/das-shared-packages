using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    internal static class ConfigureGovUkStubAuthenticationExtension
    {

        public static void AddEmployerStubAuthentication(this IServiceCollection services, string redirectUrl, string loginRedirect, string localRedirect, string cookieDomain)
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
                            var additionalClaims = await customClaims.GetClaims(new TokenValidatedContext(ctx.HttpContext,new AuthenticationScheme(CookieAuthenticationDefaults.AuthenticationScheme, "Cookie", typeof(EmployerStubAuthHandler)), new OpenIdConnectOptions(), ctx.Principal, new AuthenticationProperties() ));
                            foreach (var additionalClaim in additionalClaims)
                            {
                                if(claims.FirstOrDefault(c=>c.Type == additionalClaim.Type) == null)
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