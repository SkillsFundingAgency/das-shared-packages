using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.GovUK.SampleSite.AppStart;

public static class AddServiceRegistrationExtension
{
    public static void AddServiceRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
        services.AddAndConfigureGovUkAuthentication(configuration, new AuthRedirects
        {
            NotVerifiedRedirectUrl = "/home/NotVerified",
            LoginRedirect = "/home/AccountDetails"
        }, typeof(CustomClaims));
        services.AddGovUkAuthorization();
    }
}