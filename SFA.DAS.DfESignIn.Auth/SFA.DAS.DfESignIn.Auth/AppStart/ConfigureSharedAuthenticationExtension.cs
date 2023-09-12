using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Extensions;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    public static class ConfigureSharedAuthenticationExtension
    {
        public static void AddAuthenticationCookie(
            this AuthenticationBuilder services,
            string cookieName,
            string signedOutCallbackPath,
            string resourceEnvironmentName,
            ClientName clientName)
        {
            services.AddCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/error/403");
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.Cookie.Name = cookieName;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
                options.LogoutPath = new PathString(signedOutCallbackPath);
                var environmentAndDomain = RedirectExtension.GetEnvironmentAndDomain(resourceEnvironmentName, clientName);
                if (!string.IsNullOrEmpty(environmentAndDomain))
                {
                    options.Cookie.Domain = environmentAndDomain;
                }
            });
        }
    }
}