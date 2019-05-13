using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DfE.Example.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DfE.Example.Web.Security
{
    public static class SecurityServicesCollectionExtensions
    {
        public static void AddAuthenticationService(this IServiceCollection services, OidcConfiguration authConfiguration, IHostingEnvironment hostingEnvironment)
        {
            var authConfig = authConfiguration;

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies", options =>
            {
                options.Cookie.Name = "example-auth";

                if (!hostingEnvironment.IsDevelopment())
                {
                    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(OidcConfiguration.SessionTimeoutMinutes);
                }

                options.AccessDeniedPath = "/Error/403";
            })
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";
                options.Authority = authConfig.Authority;
                options.MetadataAddress = authConfig.MetaDataAddress;
                options.RequireHttpsMetadata = false;
                options.ResponseType = "code";
                options.ClientId = authConfig.ClientId;
                options.ClientSecret = authConfig.ClientSecret;
                options.Scope.Add("profile");
            });
        }
    }
}