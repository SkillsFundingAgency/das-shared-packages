using System.Threading.Tasks;

namespace SFA.DAS.Authorization
{
    public interface IAuthorizationHandler
    {
        Task<AuthorizationResult> CanAccessAsync(IAuthorizationContext authorizationContext, Feature feature);
    }
}