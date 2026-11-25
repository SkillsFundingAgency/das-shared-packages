using System;
using System.Security.Claims;

namespace SFA.DAS.DfESignIn.Auth.Interfaces
{
    public interface IDfESignInStubService
    {
        ClaimsPrincipal GetStubSignInClaims(
            string userId,
            Guid organisationId);
    }
}