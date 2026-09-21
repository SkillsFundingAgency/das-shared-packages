using SFA.DAS.GovUK.SampleSite.Configuration;

namespace SFA.DAS.GovUK.SampleSite.AppStart
{
    public static class AddConfigurationExtension
    {
        public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(r => new SampleSiteConfiguration
            {
                OneLoginSettingsUrl = ""
            });
        }
    }
}
