using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.GovUK.Auth.Authentication
{
    public class VerifiedIdentityAuthorizationHandler : AuthorizationHandler<VerifiedIdentityRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, VerifiedIdentityRequirement requirement)
        {
            var httpContext = context.GetHttpContext();
            var vot = httpContext?.User.Claims.FirstOrDefault(c => c.Type == "vot")?.Value;

            if (!string.IsNullOrEmpty(vot) && vot.Contains("P2"))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail(new AuthorizationFailureReason(this, AuthorizationFailureMessages.NotVerified));
            }

            return Task.CompletedTask;
        }
    }
}
