using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.GovUK.Auth.Authentication
{
    public class AccountActiveAuthorizationHandler : AuthorizationHandler<AccountActiveRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccountActiveRequirement requirement)
        {
            var decision = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthorizationDecision)?.Value;

            if (!string.Equals(decision, AuthorizationDecisions.Suspended, StringComparison.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail(new AuthorizationFailureReason(this, AuthorizationFailureMessages.NotAccountActive));
            }


            return Task.CompletedTask;
        }
    }
}