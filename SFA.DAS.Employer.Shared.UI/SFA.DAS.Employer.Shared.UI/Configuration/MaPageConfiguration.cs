using System.Collections.Generic;

namespace SFA.DAS.Employer.Shared.UI.Configuration
{
    public class MaPageConfiguration
    {
        public MaRoutes Routes { get; set; }
        public string LocalLogoutRouteName { get; set; }
        public string IdentityClientIdConfigKey { get; set; }
        public string ClientId { get; internal set; }
    }
}