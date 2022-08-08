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
        services.AddOptions();
        
        services.Configure<GovUkOidcConfiguration>(configuration.GetSection(nameof(GovUkOidcConfiguration)));
        services.AddHttpClient<IOidcService, OidcService>();
        
    }
}