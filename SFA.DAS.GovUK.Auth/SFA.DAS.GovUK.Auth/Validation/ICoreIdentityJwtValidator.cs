using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.GovUK.Auth.Validation
{
    public interface ICoreIdentityJwtValidator
    {
        ClaimsPrincipal ValidateCoreIdentity(string coreIdentityJwt);
        Task LoadDidDocument();
    }
}
