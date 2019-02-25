using System.Collections.Generic;

namespace SFA.DAS.Employer.Shared.UI.Configuration
{
    public class MaMenuConfiguration
    {
        public SiteConfiguration Ma { get; set; }
        public SiteConfiguration Commitments { get; set; }
        public SiteConfiguration Recruit { get; set; }
        public SiteConfiguration Identity { get; set; }
        public string LocalLogoutRouteName { get; set; }
        public string IdentityClientIdConfigKey { get; set; }
        public string ClientId { get; internal set; }
    }


}