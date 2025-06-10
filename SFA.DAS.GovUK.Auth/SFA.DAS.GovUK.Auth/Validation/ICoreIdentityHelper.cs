using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.GovUK.Auth.Validation
{
    internal interface ICoreIdentityHelper : IDisposable
    {
        ClaimsPrincipal ValidateCoreIdentity(string coreIdentityJwt);
        Task EnsureDidDocument();
    }
}
