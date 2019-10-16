using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System;
using System.IdentityModel.Tokens;
using System.Runtime.CompilerServices;

namespace SFA.DAS.Authentication.Extensions
{
    public static class JwtBearerAuthenticationExtensions
    {
        public static IAppBuilder UseMixedModeAuthentication(this IAppBuilder app, MixedModeAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            if (options.Logger == null)
            {
                options.Logger = new NullLogger();
            }

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.FromMinutes(5),
                    IssuerSigningKey = new   InMemorySymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(options.ApiTokenSecret)),
                    IssuerSigningTokens = (new OpenIdConnectSecurityTokenProvider(options.MetadataEndpoint, options.Logger)).SecurityTokens,
                    RequireSignedTokens = true,
                    ValidateIssuer = true,
                    ValidIssuers = options.ValidIssuers,
                    ValidateAudience = true,
                    ValidAudiences = options.ValidAudiences
                }
                ,
                TokenHandler = new DasJwtSecurityTokenHandler(options.Logger)
            });

            return app;
        }
    }
}
