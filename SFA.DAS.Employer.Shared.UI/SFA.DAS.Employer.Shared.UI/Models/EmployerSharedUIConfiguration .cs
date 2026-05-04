using System.Collections.Generic;

namespace SFA.DAS.Employer.Shared.UI.Models
{
    public class EmployerSharedUIConfiguration
    {
        public string DashboardUrl { get; set; }

        public Dictionary<string, string> LocalPorts { get; set; } = new Dictionary<string, string>();

        public string ResourceEnvironmentName { get; set; } = string.Empty;
    }
}