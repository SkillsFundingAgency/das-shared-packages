using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.GovUK.Auth.AppStart;

public static class AddAndConfigureGovUkAuthenticationExtension
{
    public static void AddAndConfigureGovUkAuthentication(this IServiceCollection services, IConfiguration configuration, string authenticationCookieName, Func<TokenValidatedContext, Task<List<Claim>>>? populateAdditionalClaims = null)
    {
        services.AddServiceRegistration(configuration);
        if (!string.IsNullOrEmpty(configuration["NoAuthEmail"]))
        {
            services.AddEmployerStubAuthentication($"{authenticationCookieName}.stub", populateAdditionalClaims);
        }
        else
        {
            services.ConfigureGovUkAuthentication(configuration, authenticationCookieName, populateAdditionalClaims);    
        }
        
    }
}