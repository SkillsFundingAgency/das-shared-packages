using System.Collections.Generic;

namespace SFA.DAS.Employer.Shared.UI.Configuration
{
    public class SiteConfiguration
    {
        public string RootUrl { get; set; }
        public Dictionary<string, string> Routes { get; set; }
    }
}