using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.GovUK.Auth.Authentication
{
    public class VerifiedIdentityFailureHandler : IAuthorizationFailureHandler
    {
        public Task<bool> HandleFailureAsync(HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult result)
        {
            var isVerifiedRequirementInPolicy =
                policy.Requirements.OfType<VerifiedIdentityRequirement>().Any();

            var isVerifiedIdentityRequirementFailed =
                result.AuthorizationFailure?.FailureReasons.Any(r => r.Message == AuthorizationFailureMessages.NotVerified) == true;

            if (isVerifiedRequirementInPolicy && isVerifiedIdentityRequirementFailed)
            {
                var returnUrl = context.Request.Path + context.Request.QueryString;
                var redirectUrl = $"/service/verify-identity?returnUrl={Uri.EscapeDataString(returnUrl)}";

                context.Response.Redirect(redirectUrl);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }

}
