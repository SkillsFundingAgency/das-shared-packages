using SFA.DAS.Configuration;

namespace SFA.DAS.UI.Activities.Domain.Configurations
{
    public class ActivitiesConfiguration : IConfiguration
    {
        public string ElasticServerBaseUrl { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}
