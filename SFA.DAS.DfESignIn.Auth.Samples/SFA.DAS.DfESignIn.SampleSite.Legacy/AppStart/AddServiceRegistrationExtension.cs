using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.AppStart;

namespace SFA.DAS.DfESignIn.SampleSite.AppStart
{
    public static class AddServiceRegistrationExtension
    {
        public static void AddServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            services.AddAndConfigureDfESignInAuthentication(configuration,
                $"{typeof(AddServiceRegistrationExtension).Assembly.GetName().Name}.Auth", typeof(CustomClaims));
        }
    }
}