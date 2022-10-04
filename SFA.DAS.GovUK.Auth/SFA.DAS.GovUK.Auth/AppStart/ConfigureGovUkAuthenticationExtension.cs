using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Interfaces;

namespace SFA.DAS.GovUK.Auth.AppStart;

internal static class ConfigureGovUkAuthenticationExtension
{
    internal static void ConfigureGovUkAuthentication(this IServiceCollection services, IConfiguration configuration, string authenticationCookieName)
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
                var govUkConfiguration = configuration.GetSection(nameof(GovUkOidcConfiguration));

                options.ClientId = govUkConfiguration["ClientId"];
                options.MetadataAddress = $"{govUkConfiguration["BaseUrl"]}/.well-known/openid-configuration";
                options.ResponseType = "code";
                options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                options.SignedOutRedirectUri = "/";
                options.SignedOutCallbackPath = "/signed-out";
                options.CallbackPath = "/sign-in";
                options.ResponseMode = string.Empty;

                options.SaveTokens = true;

                var scopes = "openid email phone".Split(' ');
                options.Scope.Clear();
                foreach (var scope in scopes)
                {
                    options.Scope.Add(scope);
                }

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


            }).AddAuthenticationCookie(authenticationCookieName);
        services
            .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
            .Configure<IOidcService, IAzureIdentityService>((options, oidcService, azureIdentityService) =>
            {
                var govUkConfiguration = configuration.GetSection(nameof(GovUkOidcConfiguration));
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    AuthenticationType = "private_key_jwt",
                    IssuerSigningKey = new KeyVaultSecurityKey(govUkConfiguration["KeyVaultIdentifier"] ,azureIdentityService.AuthenticationCallback),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    SaveSigninToken = true
                };
                options.Events.OnTokenValidated = async ctx => await oidcService.PopulateAccountClaims(ctx);
                options.Events.OnAuthorizationCodeReceived = async (ctx) =>
                {
                    var token = await oidcService.GetToken(ctx.TokenEndpointRequest);
                    if (token is {AccessToken: { }, IdToken: { }})
                    {
                        ctx.HandleCodeRedemption(token.AccessToken, token.IdToken);    
                    }
                };
            });
    }
}