using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.GovUK.Auth.Services;

public interface IStubAuthenticationService : IGovUkAuthenticationService
{
    Task<ClaimsPrincipal> GetStubSignInClaims(StubAuthUserDetails model);

    Task<GovUkUser> GetStubVerifyGovUkUser(IFormFile formFile);
}
