using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Api.Client;
using SFA.DAS.DfESignIn.Auth.Api.Models;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    internal static class ConfigureDfESignInAuthenticationExtension
    {
        private static readonly IDfEClaims _dfeClaims = new DfEClaims();

        internal static void ConfigureDfESignInAuthentication(this IServiceCollection services,
            IConfiguration configuration, string authenticationCookieName)
        {
            services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddOpenIdConnect(options =>
                {
                    options.ClientId = configuration["DfEOidcConfiguration:ClientId"];
                    options.ClientSecret = configuration["DfEOidcConfiguration:Secret"];
                    options.MetadataAddress = $"{configuration["DfEOidcConfiguration:BaseUrl"]}/.well-known/openid-configuration";
                    options.ResponseType = "code";
                    options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                    options.SignedOutRedirectUri = "/";
                    options.SignedOutCallbackPath = "/signed-out";
                    options.CallbackPath = "/sign-in";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    var scopes = "openid email profile organisation organisationid".Split(' ');
                    options.Scope.Clear();
                    foreach (var scope in scopes)
                    {
                        options.Scope.Add(scope);
                    }

                    options.SecurityTokenValidator = new JwtSecurityTokenHandler()
                    {
                        InboundClaimTypeMap = new Dictionary<string, string>(),
                        TokenLifetimeInMinutes = 90,
                        SetDefaultTimesOnTokenCreation = true
                    };
                    options.ProtocolValidator = new OpenIdConnectProtocolValidator
                    {
                        RequireSub = true,
                        RequireStateValidation = false,
                        NonceLifetime = TimeSpan.FromMinutes(60)
                    };

                    options.Events.OnRemoteFailure = c =>
                    {
                        if (c.Failure != null && c.Failure.Message.Contains("Correlation failed"))
                        {
                            c.Response.Redirect("/");
                            c.HandleResponse();
                        }

                        return Task.CompletedTask;
                    };

                    options.Events.OnSignedOutCallbackRedirect = c =>
                    {
                        c.Response.Cookies.Delete(authenticationCookieName);
                        c.Response.Redirect("/");
                        c.HandleResponse();
                        return Task.CompletedTask;
                    };

                    options.Events.OnTokenValidated = async c =>
                    {
                        await PopulateAccountsClaim(c, configuration);

                    };

                })
                .AddAuthenticationCookie(authenticationCookieName);
        }

        private static async Task PopulateAccountsClaim(TokenValidatedContext ctx, IConfiguration config)
        {
            string userId = ctx.Principal.Claims.Where(c => c.Type.Contains("sub")).Select(c => c.Value).SingleOrDefault();

            var userOrganisation = JsonConvert.DeserializeObject<Organisation>
            (
                ctx.Principal.Claims.Where(c => c.Type == "organisation")
                .Select(c => c.Value)
                .FirstOrDefault()
            );
            
            var ukPrn = userOrganisation.UkPrn ?? 10000001;

            await _dfeClaims.GetClaims(ctx, userId, userOrganisation.Id.ToString(), config);

            var displayName = ctx.Principal.Claims.FirstOrDefault(c => c.Type.Equals("given_name")).Value + " " + ctx.Principal.Claims.FirstOrDefault(c => c.Type.Equals("family_name")).Value;
            ctx.HttpContext.Items.Add(ClaimsIdentity.DefaultNameClaimType, ukPrn.ToString());
            ctx.HttpContext.Items.Add("http://schemas.portal.com/displayname", displayName);
            ctx.Principal.Identities.First().AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, ukPrn.ToString()));
            ctx.Principal.Identities.First().AddClaim(new Claim("http://schemas.portal.com/displayname", displayName));
            ctx.Principal.Identities.First().AddClaim(new Claim("http://schemas.portal.com/ukprn", ukPrn.ToString()));
            ctx.Principal.Identities.First().AddClaim(new Claim("http://schemas.portal.com/service", "DAA"));
        }
    }
}