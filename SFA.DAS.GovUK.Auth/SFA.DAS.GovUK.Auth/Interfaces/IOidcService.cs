using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.GovUK.Auth.Interfaces;

public interface IOidcService
{
    Task<Token?> GetToken(OpenIdConnectMessage openIdConnectMessage, string clientAssertion);
}