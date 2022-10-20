using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.GovUK.Auth.Services
{
    public interface IOidcService
    {
        Task<Token> GetToken(OpenIdConnectMessage openIdConnectMessage);
        Task PopulateAccountClaims(TokenValidatedContext tokenValidatedContext);
    }
}