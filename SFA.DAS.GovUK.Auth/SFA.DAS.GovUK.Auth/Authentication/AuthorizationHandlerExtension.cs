using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.GovUK.Auth.Authentication
{
    public static class AuthorizationHandlerExtension
    {
        public static HttpContext GetHttpContext(this AuthorizationHandlerContext context)
        {
            HttpContext currentContext;
            switch (context.Resource)
            {
                case HttpContext httpContext:
                    currentContext = httpContext;
                    break;
                case AuthorizationFilterContext authorizationFilterContext:
                    currentContext = authorizationFilterContext.HttpContext;
                    break;
                default:
                    currentContext = null;
                    break;
            }

            return currentContext;
        }
    }
}
