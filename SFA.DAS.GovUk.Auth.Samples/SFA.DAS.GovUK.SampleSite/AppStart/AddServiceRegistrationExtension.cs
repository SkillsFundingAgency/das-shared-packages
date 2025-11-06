using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Authentication;
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
            SignedOutRedirectUrl = "/user-signed-out",
            SuspendedRedirectUrl = "/user-suspended",
            LocalStubLoginPath = "/stub/sign-in-Stub"
        }, typeof(CustomClaims));
        services.AddGovUkAuthorization();
        services.AddValidatorsFromAssemblyContaining<SignInStubViewModelValidator>();
    }

    public static void AddGovUkAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                PolicyNames.IsAuthenticated, policy =>
                {
                    policy.RequireAuthenticatedUser();
                });

            options.AddPolicy(
                PolicyNames.IsActiveAccount, policy =>
                {
                    policy.Requirements.Add(new AccountActiveRequirement());
                    policy.RequireAuthenticatedUser();
                });

            options.AddPolicy(
                PolicyNames.IsVerified, policy =>
                {
                    policy.Requirements.Add(new AccountActiveRequirement());
                    policy.Requirements.Add(new VerifiedIdentityRequirement());
                    policy.RequireAuthenticatedUser();
                });
        });
    }
}