using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SFA.DAS.GovUK.Auth.AppStart;

internal static class ConfigureGovUkStubAuthenticationExtension
{

    public static void AddEmployerStubAuthentication(this IServiceCollection services, string authenticationCookieName)
    {
        services
            .AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, EmployerStubAuthHandler>(authenticationCookieName, _ => { })
            .AddCookie(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Events.OnSigningOut = c =>
                {
                    c.Response.Cookies.Delete(authenticationCookieName);
                    c.Response.Redirect("/");
                    return Task.CompletedTask;
                };
            });

        services.AddAuthentication(authenticationCookieName).AddAuthenticationCookie(authenticationCookieName);
    }
    
}