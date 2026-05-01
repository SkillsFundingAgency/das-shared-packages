using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Employer.Shared.UI.Extensions;
using SFA.DAS.Employer.Shared.UI.Models;

namespace SFA.DAS.Employer.Shared.UI
{
    public static class AddServiceExtensions
    {
        public static void AddEmployerUiServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(EmployerSharedUIConfiguration));
            if (!section.GetChildren().Any())
            {
                throw new Exception("Cannot find EmployerSharedUIConfiguration in configuration. Please add a section called EmployerSharedUIConfiguration with DashboardUrl property");
            }

            var config = new EmployerSharedUIConfiguration
            {
                DashboardUrl = section[nameof(EmployerSharedUIConfiguration.DashboardUrl)]
            };

            services.AddSingleton(config);
            services.AddSingleton<IExternalUrlHelper, ExternalUrlHelper>();
        }
    }
}
