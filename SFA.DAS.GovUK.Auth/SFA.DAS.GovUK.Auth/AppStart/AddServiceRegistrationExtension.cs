using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Interfaces;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.Auth.AppStart;

internal static class AddServiceRegistrationExtension
{
    internal static void AddServiceRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        if(!configuration.GetSection(nameof(GovUkOidcConfiguration)).GetChildren().Any())
        {
            throw new Exception("Cannot find GovUkOidcConfiguration in configuration. Please add a section called GovUkOidcConfiguration with BaseUrl, ClientId and KeyVaultIdentifier properties.");
        }
        services.AddOptions();
        services.Configure<GovUkOidcConfiguration>(configuration.GetSection(nameof(GovUkOidcConfiguration)));
        services.AddHttpClient<IOidcService, OidcService>();
        services.AddTransient<IAzureIdentityService, AzureIdentityService>();
        services.AddTransient<IJwtSecurityTokenService, JwtSecurityTokenService>();
    }
}