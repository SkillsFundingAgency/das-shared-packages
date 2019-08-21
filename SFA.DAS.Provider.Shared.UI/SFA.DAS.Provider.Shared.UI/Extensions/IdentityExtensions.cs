using System;
using System.Security.Claims;
using System.Security.Principal;

namespace SFA.DAS.Provider.Shared.UI.Extensions
{
    public static class IdentityExtensions
    {
        public static int? GetProviderId(this IIdentity identity)
        {
            const string claim = "http://schemas.portal.com/ukprn";
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var claimValue = claimsPrincipal.FindFirst(c => c.Type == claim)?.Value;

            if (int.TryParse(claimValue, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Unable to parse Provider Id \"{claimValue}\" from user claims");
        }
    }
}