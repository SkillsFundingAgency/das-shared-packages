using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using SFA.DAS.GovUK.Auth.Controllers.Routes;

namespace SFA.DAS.GovUK.Auth.Authentication
{
    public class VerifiedIdentityFailureHandler : IAuthorizationFailureHandler
    {
        private readonly string _verifyIdentityInformationUrl;

        public VerifiedIdentityFailureHandler()
            : this(null)
        {
        }

        public VerifiedIdentityFailureHandler(string verifyIdentityInformationUrl)
        {
            _verifyIdentityInformationUrl = verifyIdentityInformationUrl;
        }

        public Task<bool> HandleFailureAsync(HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult result)
        {
            var isVerifiedRequirementInPolicy =
                policy.Requirements.OfType<VerifiedIdentityRequirement>().Any();

            var isVerifiedIdentityRequirementFailed =
                result.AuthorizationFailure?.FailureReasons
                    .Any(r => r.Message == AuthorizationFailureMessages.NotVerified) == true;

            if (!isVerifiedRequirementInPolicy || !isVerifiedIdentityRequirementFailed)
            {
                return Task.FromResult(false);
            }

            var returnUrl = context.Request.Path + context.Request.QueryString;

            var redirectBaseUrl = string.IsNullOrWhiteSpace(_verifyIdentityInformationUrl)
                ? ServiceRoutes.Paths.VerifyIdentity.ServiceControllerPath()
                : _verifyIdentityInformationUrl;

            var redirectUrl =
                $"{redirectBaseUrl}?returnUrl={Uri.EscapeDataString(returnUrl)}";

            context.Response.Redirect(redirectUrl);

            return Task.FromResult(true);
        }
    }
}
