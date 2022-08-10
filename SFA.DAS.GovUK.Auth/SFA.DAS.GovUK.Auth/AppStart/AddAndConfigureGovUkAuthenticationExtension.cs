using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.GovUK.Auth.AppStart;

public static class AddAndConfigureGovUkAuthenticationExtension
{
    public static void AddAndConfigureGovUkAuthentication(this IServiceCollection services, IConfiguration configuration, string authenticationCookieName)
    {
        services.AddServiceRegistration(configuration);
        services.ConfigureGovUkAuthentication(configuration, authenticationCookieName);
    }
}