using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.GovUK.Auth.Extensions;
using SFA.DAS.GovUK.Auth.Helper;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.GovUK.Auth.Validation
{
    internal class ValidateCoreIdentityJwtClaimAction : ClaimAction
    {
        private readonly ICoreIdentityJwtValidator _coreIdentityHelper;

        public ValidateCoreIdentityJwtClaimAction(ICoreIdentityJwtValidator coreIdentityHelper) :
            base(UserInfoClaims.CoreIdentityJWT.GetDescription(), valueType: JsonClaimValueTypes.Json)
        {
            _coreIdentityHelper = coreIdentityHelper;
        }

        public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
        {
            if (!userData.TryGetProperty(UserInfoClaims.CoreIdentityJWT.GetDescription(), out var identityJwtElement))
            {
                // the core identity claim may not be present, even if it was requested
                return;
            }

            var token = identityJwtElement.GetString()!;

            var coreIdentityPrincipal = _coreIdentityHelper.ValidateCoreIdentity(token);

            if (coreIdentityPrincipal.FindFirstValue("sub") != identity.FindFirst(ClaimTypes.NameIdentifier)?.Value)
            {
                throw new SecurityTokenException("The 'sub' claim in the core identity JWT does not match the 'sub' claim from the ID token.");
            }

            var coreIdentity = JsonSerializer.Deserialize<GovUkCoreIdentityJwt>(
                JsonSerializer.Serialize(token));

            if (coreIdentity?.Vc?.CredentialSubject == null)
            {
                throw new SecurityTokenException("The core identity contains no credential subject.");
            }

            var latestName = CoreIdentityJwtClaimHelper
                .GetLatestNameFrom(coreIdentity);

            if (!string.IsNullOrWhiteSpace(latestName?.GivenName))
            {
                identity.AddOrReplaceClaim(
                    ClaimTypes.GivenName, latestName.GivenName, _ => false);
            }

            if (!string.IsNullOrWhiteSpace(latestName?.FamilyName))
            {
                identity.AddOrReplaceClaim(
                    ClaimTypes.Surname, latestName.FamilyName, _ => false);
            }

            if (!string.IsNullOrWhiteSpace(latestName?.FullName))
            {
                identity.AddOrReplaceClaim(
                    ClaimTypes.Name, latestName.FullName, _ => false);
            }

            // the JWT claim was present and its subject were validated successfully
            identity.AddOrReplaceClaim(
                GovUkUserClaimTypes.VerifiedIdentity, "true", _ => false);
        }
    }
}
