using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure.Configuration;

namespace SFA.DAS.Api.Common.AppStart;

public static class AddAuthenticationExtension
{
    public static void AddAuthentication(
        this IServiceCollection services,
        AzureActiveDirectoryConfiguration config, Dictionary<string, string> policies)
    {
        services.AddAuthorization(o =>
        {
            foreach (var policyName in policies)
            {
                o.AddPolicy(policyName.Key, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(policyName.Value);
                });
            }
        });

        services.AddAuthentication(auth => { auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
            .AddJwtBearer(auth =>
            {
                auth.Authority = $"https://login.microsoftonline.com/{config.Tenant}";
                auth.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidAudiences = config.Identifier.Split(",")
                };
            });
        services.AddSingleton<IClaimsTransformation, AzureAdScopeClaimTransformation>();
    }
}