using System.Collections.Generic;

namespace SFA.DAS.Employer.Shared.UI.Models
{
    public class EmployerSharedUIConfiguration
    {
        public string DashboardUrl { get; set; }

        public Dictionary<string, string> LocalPorts { get; set; }

        public string ResourceEnvironmentName { get; set; }
    }
}