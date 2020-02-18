
using System.Security.Principal;

namespace SFA.DAS.MA.Shared.UI.Configuration
{
    public interface IUserContext
    {
        string HashedAccountId { get; set; }
        IPrincipal User { get; set; }
    }
}
