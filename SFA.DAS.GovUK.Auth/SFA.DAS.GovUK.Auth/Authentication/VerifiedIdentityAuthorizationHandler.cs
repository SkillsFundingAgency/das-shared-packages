using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.GovUK.Auth.Authentication
{
    public class VerifiedIdentityAuthorizationHandler : AuthorizationHandler<VerifiedIdentityRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, VerifiedIdentityRequirement req)
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
                var vot = currentContext.User.Claims.FirstOrDefault(c => c.Type == "vot")?.Value;
                if (!string.IsNullOrEmpty(vot) && vot.Contains("P2"))
                {
                    context.Succeed(req);
                    return Task.CompletedTask;
                }

                var originalUrl = currentContext.Request.Path + currentContext.Request.QueryString;
                var target = $"/service/verify-identity?returnUrl={Uri.EscapeDataString(originalUrl)}";
                currentContext.Response.Redirect(target);
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}
