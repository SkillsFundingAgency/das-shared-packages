using Microsoft.AspNetCore.Hosting;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.EmployerUrlHelper.Sample.Startup
{
    public static class ConfigurationStartup
    {
        public static IWebHostBuilder ConfigureAppConfiguration(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureAppConfiguration(c => c.AddAzureTableStorage("SFA.DAS.EmployerUrlHelper"));
        }
    }
}