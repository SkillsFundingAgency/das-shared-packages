using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Api;
using SFA.DAS.DfESignIn.Auth.Api.Client;
using SFA.DAS.DfESignIn.Auth.Api.Models;
using SFA.DAS.DfESignIn.Auth.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    internal static class ConfigureDfESignInAuthenticationExtension
    {
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
                }).AddOpenIdConnect(options =>
                {
                    var dsiConfiguration = configuration.GetSection("DfEOidcConfiguration").Get<DfEOidcConfiguration>();

                    options.ClientId = dsiConfiguration.ClientId;
                    options.ClientSecret = dsiConfiguration.Secret;
                    options.MetadataAddress = $"{dsiConfiguration.BaseUrl}/.well-known/openid-configuration";
                    options.ResponseType = dsiConfiguration.ResponseType;
                    options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                    options.SignedOutRedirectUri = "/";
                    options.SignedOutCallbackPath = "/signed-out";
                    options.CallbackPath = "/sign-in";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    var scopes = dsiConfiguration.Scopes.Split(' ');
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

                }).AddAuthenticationCookie(authenticationCookieName);
        }

        private static async Task PopulateAccountsClaim(TokenValidatedContext ctx, IConfiguration config)
        {
            var dsiConfiguration = config.GetSection("DfEOidcConfiguration").Get<DfEOidcConfiguration>();

            string userId = ctx.Principal.Claims.Where(c => c.Type.Contains("sub")).Select(c => c.Value).SingleOrDefault();

            var userOrganisation = JsonConvert.DeserializeObject<Organisation>
            (
                ctx.Principal.Claims.Where(c => c.Type == "organisation")
                .Select(c => c.Value)
                .FirstOrDefault()
            );
            
            var ukPrn = userOrganisation.UkPrn ?? 10000531;

            await DfEPublicApi(ctx, userId, userOrganisation.Id.ToString(), config);

            var displayName = ctx.Principal.Claims.FirstOrDefault(c => c.Type.Equals("given_name")).Value + " " + ctx.Principal.Claims.FirstOrDefault(c => c.Type.Equals("family_name")).Value;
            ctx.HttpContext.Items.Add(ClaimsIdentity.DefaultNameClaimType, ukPrn);
            ctx.HttpContext.Items.Add("http://schemas.portal.com/displayname", displayName);
            ctx.Principal.Identities.First().AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, ukPrn.ToString()));
            ctx.Principal.Identities.First().AddClaim(new Claim("http://schemas.portal.com/displayname", displayName));
            ctx.Principal.Identities.First().AddClaim(new Claim("http://schemas.portal.com/ukprn", ukPrn.ToString()));
        }


        private static async Task DfEPublicApi(TokenValidatedContext ctx, string userId, string userOrgId, IConfiguration config)
        {
            var clientFactory = new DfESignInClientFactory(config);
            DfESignInClient dfeSignInClient = clientFactory.CreateDfESignInClient(userId, userOrgId);
            HttpResponseMessage response = await dfeSignInClient.HttpClient.GetAsync(dfeSignInClient.TargetAddress);

            string stream = "";
            if (response.IsSuccessStatusCode)
            {
                stream = await response.Content.ReadAsStringAsync();

                var apiServiceResponse = JsonConvert.DeserializeObject<ApiServiceResponse>(stream);
                var roleClaims = new List<Claim>();
                foreach (var role in apiServiceResponse.Roles)
                {
                    if (role.Status.Id.Equals(1))
                    {
                        roleClaims.Add(new Claim("rolecode", role.Code, ClaimTypes.Role, ctx.Options.ClientId));
                        roleClaims.Add(new Claim("roleId", role.Id.ToString(), ClaimTypes.Role, ctx.Options.ClientId));
                        roleClaims.Add(new Claim("roleName", role.Name, ClaimTypes.Role, ctx.Options.ClientId));
                        roleClaims.Add(new Claim("rolenumericid", role.NumericId.ToString(), ClaimTypes.Role, ctx.Options.ClientId));
                    }
                }

                var roleIdentity = new ClaimsIdentity(roleClaims);
                ctx.Principal.AddIdentity(roleIdentity);
            }
        }
    }
}