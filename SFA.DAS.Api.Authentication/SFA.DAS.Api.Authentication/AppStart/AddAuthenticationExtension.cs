using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Api.Authentication.Configuration;
using SFA.DAS.Api.Authentication.Infrastructure.Configuration;

namespace SFA.DAS.Api.Authentication.AppStart
{
    public static class AddAuthenticationExtension
    {
        public static void AddAuthentication(
            this IServiceCollection services,
            AzureActiveDirectoryConfiguration config)
        {
            services.AddAuthorization(o =>
            {
                o.AddPolicy("default", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("APIM");
                });
            });

            services.AddAuthentication(auth => { auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
                .AddJwtBearer(auth =>
                {
                    auth.Authority =
                        $"https://login.microsoftonline.com/{config.Tenant}";
                    auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidAudiences = new List<string>
                        {
                            config.Identifier
                        }
                    };
                });

            services.AddSingleton<IClaimsTransformation, AzureAdScopeClaimTransformation>();
        }
    }
}