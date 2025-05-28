using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.GovUK.Auth.Authentication
{
    public class RedirectOnVerifiedIdentityFailedResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _default = new();

        public async Task HandleAsync(RequestDelegate next, HttpContext context,
                                      AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            var isVerifiedIdentityRequirementFailed =
                policy.Requirements.OfType<VerifiedIdentityRequirement>().Any() &&
                !authorizeResult.Succeeded;

            if (isVerifiedIdentityRequirementFailed &&
                context.Response.StatusCode == StatusCodes.Status302Found)
            {
                // preserve redirect done by VerifiedIdentityAuthorizationHandler
                return;
            }

            await _default.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}