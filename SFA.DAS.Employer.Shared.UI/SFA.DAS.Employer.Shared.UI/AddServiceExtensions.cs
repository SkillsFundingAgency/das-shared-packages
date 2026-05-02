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
           services.AddOptions<EmployerSharedUIConfiguration>()
                .Bind(configuration.GetSection("EmployerSharedUIConfiguration"))
                .Validate(config => !string.IsNullOrEmpty(config.DashboardUrl), "DashboardUrl must be supplied in configuration")
                .ValidateOnStart();

            services.AddSingleton<IExternalUrlHelper, ExternalUrlHelper>();
        }
    }
}
