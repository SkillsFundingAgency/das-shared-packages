using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace SFA.DAS.GovUK.Auth.AppStart;

internal static class ConfigureGovUkStubAuthenticationExtension
{
    
    public static void AddEmployerStubAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication("Employer-stub").AddScheme<AuthenticationSchemeOptions, EmployerStubAuthHandler>(
            "Employer-stub",
            options => { });
    }
    
}