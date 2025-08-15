using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.GovUK.Auth.Authentication
{
    public class AccountActiveAuthorizationHandler : AuthorizationHandler<AccountActiveRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AccountActiveRequirement requirement)
        {
            HttpContext currentContext;
            switch (context.Resource)
            {
                case HttpContext resource:
                    currentContext = resource;
                    break;
                case AuthorizationFilterContext authorizationFilterContext:
                    currentContext = authorizationFilterContext.HttpContext;
                    break;
                default:
                    currentContext = null;
                    break;
            }

            if (currentContext != null)
            {
                var isAccountSuspended = context.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.AuthorizationDecision))?.Value;
                if (isAccountSuspended != null && isAccountSuspended.Equals("Suspended", StringComparison.CurrentCultureIgnoreCase))
                {
                    var authResult = await currentContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    var props = authResult?.Properties;

                    string redirectPath = "/error/403";
                    if (props?.Items != null && props.Items.TryGetValue("suspended_redirect", out var fromCookie) && !string.IsNullOrWhiteSpace(fromCookie))
                    {
                        redirectPath = fromCookie;
                    }

                    currentContext.Response.Redirect(redirectPath);
                }
            }
            
            context.Succeed(requirement);
        }
    }
}