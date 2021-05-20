using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;

namespace SFA.DAS.Provider.Shared.UI.Startup
{
    public static class AddServiceExtensions
    {
        public static void AddProviderUiServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            if(!configuration.GetSection(nameof(ProviderSharedUIConfiguration)).GetChildren().Any())
            {
                throw new Exception("Cannot find ProviderSharedUIConfiguration in configuration. Please add a section called ProviderSharedUIConfiguration with DashboardUrl property");
            }
            
            services.Configure<ProviderSharedUIConfiguration>(configuration.GetSection(nameof(ProviderSharedUIConfiguration)));
            services.AddSingleton(cfg => cfg.GetService<IOptions<ProviderSharedUIConfiguration>>().Value);
            services.AddSingleton<IExternalUrlHelper, ExternalUrlHelper>();
        }
    }
}