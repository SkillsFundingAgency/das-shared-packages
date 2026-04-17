using System;
using System.Security.Claims;

namespace SFA.DAS.GovUK.Auth.Extensions
{
    public static class ClaimsIdentityExtensions
    {
        public static bool AddOrReplaceClaim(
            this ClaimsIdentity identity,
            string type,
            string newValue,
            Func<Claim, bool> keepExisting)
        {
            var existing = identity.FindFirst(type);
            if (existing == null || !keepExisting(existing))
            {
                if (existing != null)
                {
                    identity.RemoveClaim(existing);
                }

                identity.AddClaim(new Claim(type, newValue));
                return true;
            }

            return false;
        }
    }
}