using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.GovUK.Auth.Services
{
    public interface IGovUkAuthenticationService
    {
        Task<Token> GetToken(OpenIdConnectMessage openIdConnectMessage);
        Task PopulateAccountClaims(TokenValidatedContext tokenValidatedContext);
        Task<GovUkUser> GetAccountDetails(string accessToken);
        Task<IActionResult> ChallengeWithVerifyAsync(string returnUrl, Controller controller);
    }
}