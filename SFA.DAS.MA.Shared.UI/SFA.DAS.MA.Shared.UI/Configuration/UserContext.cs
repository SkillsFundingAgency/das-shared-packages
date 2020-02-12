using System.Security.Principal;

namespace SFA.DAS.MA.Shared.UI.Configuration
{
    public class UserContext : IUserContext
    {
        public string HashedAccountId { get; set; }
        public IPrincipal User { get; set; }
    }
}
