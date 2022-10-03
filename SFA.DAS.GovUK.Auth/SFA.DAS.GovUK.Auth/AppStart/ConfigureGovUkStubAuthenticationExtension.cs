using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SFA.DAS.GovUK.Auth.Configuration;

namespace SFA.DAS.GovUK.Auth.AppStart;

internal static class ConfigureGovUkStubAuthenticationExtension
{
    
    public static void AddEmployerStubAuthentication(this IServiceCollection services, IConfiguration configuration, string authenticationCookieName)
    {
        services.AddAuthentication("Employer-stub").AddScheme<AuthenticationSchemeOptions, EmployerStubAuthHandler>(
                "Employer-stub",
                options => { })
            .AddOpenIdConnect(options =>
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
            });
            services.AddAuthenticationCookie(authenticationCookieName);
    }
    
}