using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.GovUK.Auth.Authentication
{

    public class AccountActiveFailureHandler : IAuthorizationFailureHandler
    {
        public async Task<bool> HandleFailureAsync(HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult result)
        {
            var failed = policy.Requirements.OfType<AccountActiveRequirement>().Any() &&
                         result.AuthorizationFailure?.FailureReasons.Any(r => r.Message == AuthorizationFailureMessages.NotAccountActive) == true;

            if (!failed) return false;

            var authResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var props = authResult?.Properties;

            var redirectPath = "/error/403";

            if (props?.Items != null &&
                props.Items.TryGetValue("suspended_redirect", out var fromCookie) &&
                !string.IsNullOrWhiteSpace(fromCookie))
            {
                redirectPath = fromCookie;
            }

            context.Response.Redirect(redirectPath);
            return true;
        }
    }
}