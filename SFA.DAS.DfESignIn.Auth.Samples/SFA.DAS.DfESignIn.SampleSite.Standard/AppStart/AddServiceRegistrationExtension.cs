using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.Provider.Shared.UI.Startup;

namespace SFA.DAS.DfESignIn.SampleSite.Standard.AppStart
{
    public class CustomServiceRole : ICustomServiceRole
    {
        public string RoleClaimType => "http://schemas.portal.com/service";
        public CustomServiceRoleValueType RoleValueType  => CustomServiceRoleValueType.Code;
    }

    public static class AddServiceRegistrationExtension
    {
        public static void AddServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            services.AddAndConfigureDfESignInAuthentication(configuration,
                "SFA.DAS.ProviderApprenticeshipService",
                typeof(CustomServiceRole),
                ClientName.ServiceAdmin,
                "/signout",
                "");
            services.AddProviderUiServiceRegistration(configuration);
        }
    }
}