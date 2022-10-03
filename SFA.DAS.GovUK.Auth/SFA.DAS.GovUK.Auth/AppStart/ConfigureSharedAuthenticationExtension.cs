using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    public static class ConfigureSharedAuthenticationExtension
    {
        public static void AddAuthenticationCookie(this IServiceCollection services,
            string cookieName)
        {
            services.AddAuthentication().AddCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/error/403");
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.Cookie.Name = cookieName;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
                options.LogoutPath = "/home/signed-out";
            });
        }
    }
}