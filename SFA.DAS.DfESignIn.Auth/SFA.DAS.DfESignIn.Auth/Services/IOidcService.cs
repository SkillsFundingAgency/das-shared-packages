using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.DfESignIn.Auth.Models;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.Services
{
    public interface IOidcService
    {
        Task<Token> GetToken(OpenIdConnectMessage openIdConnectMessage);
        Task PopulateAccountClaims(TokenValidatedContext tokenValidatedContext);
    }
}