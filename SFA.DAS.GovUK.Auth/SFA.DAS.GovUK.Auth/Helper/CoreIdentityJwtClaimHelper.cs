using System;
using System.Linq;
using System.Text.Json;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.GovUK.Auth.Helper
{
    internal static class CoreIdentityJwtClaimHelper
    {
        internal sealed class LatestName
        {
            public string FullName { get; init; }
            public string GivenName { get; init; }
            public string FamilyName { get; init; }
        }

        public static LatestName GetLatestNameFromJwtClaim(string claimValue)
        {
            if (string.IsNullOrWhiteSpace(claimValue))
            {
                return null;
            }

            var coreIdentityJwt = JsonSerializer.Deserialize<GovUkCoreIdentityJwt>(
                JsonSerializer.Serialize(claimValue));

            var historicalNames = coreIdentityJwt?.Vc?.CredentialSubject?.GetHistoricalNames();

            if (historicalNames == null || !historicalNames.Any())
            {
                return null;
            }

            var now = DateTime.UtcNow;

            var latestName = historicalNames
                .Where(n =>
                    n.ValidFrom <= now && n.ValidUntil >= now)
                .OrderByDescending(n => n.ValidFrom)
                .ThenByDescending(n => n.ValidUntil)
                .FirstOrDefault()
                ?? historicalNames
                    .OrderByDescending(n => n.ValidFrom)
                    .ThenByDescending(n => n.ValidUntil)
                    .FirstOrDefault();

            var givenName = latestName?.GivenNames?.Trim() ?? string.Empty;
            var familyName = latestName?.FamilyNames?.Trim() ?? string.Empty;
            var fullName = string.Join(" ", new[] { givenName, familyName }.Where(x => !string.IsNullOrWhiteSpace(x)));

            return new LatestName
            {
                FullName = string.IsNullOrWhiteSpace(fullName) ? null : fullName,
                GivenName = string.IsNullOrWhiteSpace(givenName) ? null : givenName,
                FamilyName = string.IsNullOrWhiteSpace(familyName) ? null : familyName
            };
        }
    }
}