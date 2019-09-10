using System.Collections.Generic;

namespace SFA.DAS.Employer.Shared.UI.Configuration
{
    public class MaPageConfiguration
    {

        public bool ShowHomeOnly { get; set; } = false;

        public MaRoutes Routes { get; set; }
        public string AccountsOidcClientId { get; set; }
        public string LocalLogoutRouteName { get; set; }
    }
}