using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Employer.Shared.UI.Extensions;
using SFA.DAS.Employer.Shared.UI.Models;

namespace SFA.DAS.Employer.Shared.UI
{
    public static class AddServiceExtensions
    {
        public static void AddEmployerUiServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            if (!configuration.GetSection("EmployerSharedUIConfiguration").GetChildren().Any())
            {
                throw new Exception("Cannot find EmployerSharedUIConfiguration in configuration. Please add a section called EmployerSharedUIConfiguration with DashboardUrl property");
            }

            services.Configure<EmployerSharedUIConfiguration>(configuration.GetSection("EmployerSharedUIConfiguration"));
            services.AddSingleton((IServiceProvider cfg) => cfg.GetService<IOptions<EmployerSharedUIConfiguration>>().Value);
            services.AddSingleton<IExternalUrlHelper, ExternalUrlHelper>();
        }
    }
}
