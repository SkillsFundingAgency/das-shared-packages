using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.SampleSite.Validators;

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
            SignedOutRedirectUrl = "/signed-out",
            LocalStubLoginPath = "/services/sign-in-Stub"
        }, typeof(CustomClaims));
        services.AddGovUkAuthorization();
        services.AddValidatorsFromAssemblyContaining<SignInStubViewModelValidator>();
    }
}