using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.GovUK.Auth.Authentication
{
    public interface IAuthorizationFailureHandler
    {
        Task<bool> HandleFailureAsync(HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult result);
    }
}
