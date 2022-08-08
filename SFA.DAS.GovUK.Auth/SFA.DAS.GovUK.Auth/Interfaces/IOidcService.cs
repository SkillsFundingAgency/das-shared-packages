using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.GovUK.Auth.Interfaces;

internal interface IOidcService
{
    Task<Token?> GetToken(OpenIdConnectMessage openIdConnectMessage, string clientAssertion);
}