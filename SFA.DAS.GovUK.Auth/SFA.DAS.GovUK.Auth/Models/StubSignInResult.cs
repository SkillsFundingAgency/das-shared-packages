using System.Security.Claims;

namespace SFA.DAS.GovUK.Auth.Models
{
    public class StubSignInResult
    {
        public ClaimsPrincipal Principal { get; set; }
        public bool ResponseHandled { get; set; }
    }
}
