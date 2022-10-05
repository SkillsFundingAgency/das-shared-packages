using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.GovUK.Auth.Interfaces;

public interface IOidcService
{
    Task<Token?> GetToken(OpenIdConnectMessage? openIdConnectMessage);
    Task PopulateAccountClaims(TokenValidatedContext tokenValidatedContext, Func<TokenValidatedContext, Task<List<Claim>>>? populateAdditionalClaims = null);
}