using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.GovUK.Auth.Authentication
{
    public class ChainedAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
        private readonly IEnumerable<IAuthorizationFailureHandler> _handlers;

        public ChainedAuthorizationResultHandler(IEnumerable<IAuthorizationFailureHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task HandleAsync(RequestDelegate next, HttpContext context,
            AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (!authorizeResult.Succeeded && context.User.Identity?.IsAuthenticated == true)
            {
                foreach (var handler in _handlers)
                {
                    if (await handler.HandleFailureAsync(context, policy, authorizeResult))
                    {
                        return;
                    }
                }
            }

            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
