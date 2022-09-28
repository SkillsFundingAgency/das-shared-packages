using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.GovUK.Auth.AppStart;

public static class AddAndConfigureGovUkAuthenticationExtension
{
    public static void AddAndConfigureGovUkAuthentication(this IServiceCollection services, IConfiguration configuration, string authenticationCookieName, bool useStub = false)
    {
        services.AddServiceRegistration(configuration);
        if (useStub)
        {
            services.AddEmployerStubAuthentication();
        }
        else
        {
            services.ConfigureGovUkAuthentication(configuration, authenticationCookieName);    
        }
        
    }
}