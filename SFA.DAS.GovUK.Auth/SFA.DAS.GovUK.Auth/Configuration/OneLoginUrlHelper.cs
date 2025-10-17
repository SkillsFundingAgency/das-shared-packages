using System;

namespace SFA.DAS.GovUK.Auth.Configuration
{
    internal static class OneLoginUrlHelper
    {
        internal static string GetMetadataAddress(string baseUrl)
        {
            ArgumentNullException.ThrowIfNull(baseUrl);

            return $"{baseUrl}/.well-known/openid-configuration";
        }

        internal static string GetClientAssertionJwtAudience(string baseUrl)
        {
            ArgumentNullException.ThrowIfNull(baseUrl);

            return $"{baseUrl}/token";
        }

        internal static string GetCoreIdentityClaimIssuer(string baseUrl)
        {
            ArgumentNullException.ThrowIfNull(baseUrl);

            return $"{baseUrl}/".Replace("oidc", "identity");
        }

        internal static string GetDidEndpoint(string baseUrl)
        {
            ArgumentNullException.ThrowIfNull(baseUrl);

            return $"{baseUrl}/.well-known/did.json".Replace("oidc", "identity");
        }
    }
}
