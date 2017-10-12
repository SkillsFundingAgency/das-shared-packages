using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Configuration;

namespace SFA.DAS.UI.Activities.Domain.Configurations
{
    public class ActivitiesConfiguration : IConfiguration
    {
        public string ElasticServerBaseUrl { get; set; } = "\"http://localhost:9200\"";
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}
