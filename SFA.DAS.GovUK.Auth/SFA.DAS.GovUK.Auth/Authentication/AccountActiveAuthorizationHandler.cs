using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SFA.DAS.GovUK.Auth.Extensions;

namespace SFA.DAS.GovUK.Auth.Authentication
{
    public class AccountActiveAuthorizationHandler : AuthorizationHandler<AccountActiveRequirement>
    {
        private readonly IConfiguration _configuration;

        public AccountActiveAuthorizationHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccountActiveRequirement requirement)
        {
            if (context.Resource is HttpContext httpContext)
            {
                var isAccountSuspended = context.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.AuthorizationDecision))?.Value;
                if (isAccountSuspended != null && isAccountSuspended.Equals("Suspended", StringComparison.CurrentCultureIgnoreCase))
                {
                    httpContext.Response.Redirect(RedirectExtension.GetAccountSuspendedRedirectUrl(_configuration["ResourceEnvironmentName"]));
                }
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}