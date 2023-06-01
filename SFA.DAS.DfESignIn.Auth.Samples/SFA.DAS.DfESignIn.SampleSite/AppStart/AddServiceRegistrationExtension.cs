using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.Provider.Shared.UI.Startup;

namespace SFA.DAS.DfESignIn.SampleSite.AppStart;

public static class AddServiceRegistrationExtension
{
    // For testing purpose we have mentioned the ClientName as "QA".
    // This client name value has to be same as OpenID Connect Client Id
    // https://test-manage.signin.education.gov.uk/services/9F92718F-FCC5-4CDA-8F80-EEA8004FE089/service-configuration
    private const string ClientName = "QA";
    private const string SignedOutCallbackPath = "/signed-out";

    public static void AddServiceRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
        services.AddAndConfigureDfESignInAuthentication(
            configuration,
            $"{typeof(AddServiceRegistrationExtension).Assembly.GetName().Name}.Auth",
            typeof(CustomServiceRole),
            ClientName, 
            SignedOutCallbackPath);
        services.AddProviderUiServiceRegistration(configuration);
    }
}