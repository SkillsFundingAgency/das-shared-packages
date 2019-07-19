using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Provider.Idams.Stub.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services necessary for authentication using provider idams stub
        /// </summary>
        /// <param name="services"></param>
        /// <param name="cookieAuthenticationOptions"></param>
        /// <param name="events"></param>
        /// <returns></returns>
        public static IServiceCollection AddProviderIdamsStubAuthentication(this IServiceCollection services, Action<CookieAuthenticationOptions> cookieAuthenticationOptions, OpenIdConnectEvents events = null)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(cookieAuthenticationOptions)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = "https://das-at-pidams-as.azurewebsites.net/";
                    options.ClientId = "openIdConnectClient";
                    options.Scope.Add("openid");
                    options.Scope.Add("idams");
                    options.ResponseType = "id_token";
                    options.UseTokenLifetime = false;
                    options.RequireHttpsMetadata = false;
                    options.Events = events;
                });

            return services;
        }

    }
}
