using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Extensions;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    public static class ConfigureSharedAuthenticationExtension
    {
        /// <summary>
        /// Registers an authentication cookie using the legacy ClientName domain mapping.
        /// </summary>
        /// <remarks>
        /// Cookie domains are calculated from environment names and the package-owned
        /// ClientName enum. This requires the shared package to know individual service
        /// hostnames. Use the overload accepting cookieDomain to supply the domain
        /// from application configuration.
        /// </remarks>
        [Obsolete(
            "Use the overload accepting cookieDomain. " +
            "This overload calculates service domains from the package-owned ClientName " +
            "enum instead of using an explicitly configured domain.")]
        public static void AddAuthenticationCookie(
            this AuthenticationBuilder services,
            string cookieName,
            string signedOutCallbackPath,
            string resourceEnvironmentName,
            ClientName clientName)
        {
            var cookieDomain = RedirectExtension.GetEnvironmentAndDomain(
                resourceEnvironmentName,
                clientName);

            services.AddAuthenticationCookie(
                cookieName,
                signedOutCallbackPath,
                cookieDomain);
        }

        public static void AddAuthenticationCookie(
            this AuthenticationBuilder services,
            string cookieName,
            string signedOutCallbackPath,
            string cookieDomain)
        {
            services.AddCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/error/403");
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.Cookie.Name = cookieName;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
                options.LogoutPath = new PathString(signedOutCallbackPath);

                if (!string.IsNullOrEmpty(cookieDomain))
                {
                    options.Cookie.Domain = cookieDomain;
                }
            });
        }
    }
}