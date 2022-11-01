using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Models;
using SFA.DAS.DfESignIn.Auth.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using SFA.DAS.DfESignIn.Auth.Api;
using SFA.DAS.DfESignIn.Auth.Api.Models;

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
            services
                .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
                .Configure<IOidcService, IAzureIdentityService, ICustomClaims, IOptions<DfEOidcConfiguration>>(
                    (options, oidcService, azureIdentityService, customClaims, config) =>
                    {
                        var dsiConfiguration = config.Value;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            AuthenticationType = "private_key_jwt",
                            IssuerSigningKey = new KeyVaultSecurityKey(dsiConfiguration.KeyVaultIdentifier,
                                azureIdentityService.AuthenticationCallback),
                            ValidateIssuerSigningKey = true,
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            SaveSigninToken = true
                        };
                    });
        }

        private static async Task PopulateAccountsClaim(TokenValidatedContext ctx, IConfiguration config)
        {
            var dsiConfiguration = config.GetSection("DfEOidcConfiguration").Get<DfEOidcConfiguration>();

            string userId = ctx.Principal.Claims.Where(c => c.Type.Contains("sub")).Select(c => c.Value).SingleOrDefault();

            var organisationClaim = ctx.Principal.Claims
                .First(c => c.Type.Equals("organisation"))
                .Value;

            var userOrganization = JsonConvert.DeserializeObject<Organisation>
            (
                ctx.Principal.Claims.Where(c => c.Type == "organisation")
                .Select(c => c.Value)
                .FirstOrDefault()
            );

            var value = JsonConvert.DeserializeObject<DfESignInUser>(organisationClaim);
            var ukPrn = value.Ukprn ?? 10000531;

            // Create DfE Signin Client
            var clientFactory = new DfESignInClientFactory(config);
            DfESignInClient dfeSignInClient = clientFactory.CreateDfESignInClient(userId, userOrganization.Id.ToString());

            // Call the API
            HttpResponseMessage response = await dfeSignInClient.HttpClient.GetAsync(new Uri(string.Format("{0}/users/{1}/organisations", dsiConfiguration.APIServiceUrl, userId)));
            if (response.IsSuccessStatusCode)
            {
                //Read result
                string stuff = await response.Content.ReadAsStringAsync();
            }

            // TODO 
            var displayName = ctx.Principal.Claims.FirstOrDefault(c => c.Type.Equals("given_name")).Value + " " + ctx.Principal.Claims.FirstOrDefault(c => c.Type.Equals("family_name")).Value;
            ctx.HttpContext.Items.Add(ClaimsIdentity.DefaultNameClaimType, ukPrn);
            ctx.HttpContext.Items.Add("http://schemas.portal.com/displayname", displayName);
            ctx.Principal.Identities.First().AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, ukPrn.ToString()));
            ctx.Principal.Identities.First().AddClaim(new Claim("http://schemas.portal.com/displayname", displayName));
            ctx.Principal.Identities.First().AddClaim(new Claim("http://schemas.portal.com/ukprn", ukPrn.ToString()));
        }
    }
}