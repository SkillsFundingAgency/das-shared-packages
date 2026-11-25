using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.Provider.Shared.UI.Startup;

namespace SFA.DAS.DfESignIn.SampleSite.AppStart
{
    public static class AddServiceRegistrationExtension
    {
        private const string SignedOutCallbackPath = "/signed-out";

        public static void AddServiceRegistration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddSingleton<
                IActionContextAccessor,
                ActionContextAccessor>();

            services.AddSingleton<
                IUrlHelperFactory,
                UrlHelperFactory>();

            services.AddScoped<ICustomClaims, CustomClaims>();

            services.AddAndConfigureDfESignInAuthentication(
                configuration,
                $"{typeof(AddServiceRegistrationExtension).Assembly.GetName().Name}.Auth",
                typeof(CustomServiceRole),
                "DfEOidcConfiguration_Assessor",
                SignedOutCallbackPath);

            services.AddProviderUiServiceRegistration(configuration);
        }
    }
}