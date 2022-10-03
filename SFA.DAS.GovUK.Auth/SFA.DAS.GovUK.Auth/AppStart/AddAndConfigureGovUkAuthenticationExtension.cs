using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.GovUK.Auth.AppStart;

public static class AddAndConfigureGovUkAuthenticationExtension
{
    public static void AddAndConfigureGovUkAuthentication(this IServiceCollection services, IConfiguration configuration, string authenticationCookieName)
    {
        services.AddServiceRegistration(configuration);
        if (!string.IsNullOrEmpty(configuration["NoAuthEmail"]))
        {
            services.AddEmployerStubAuthentication(authenticationCookieName);
        }
        else
        {
            services.ConfigureGovUkAuthentication(configuration, authenticationCookieName);    
        }
        
    }
}