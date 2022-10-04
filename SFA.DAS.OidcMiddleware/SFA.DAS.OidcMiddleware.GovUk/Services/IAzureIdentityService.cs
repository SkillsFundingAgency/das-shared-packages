using System.Threading.Tasks;

namespace SFA.DAS.OidcMiddleware.GovUk.Services
{
    public interface IAzureIdentityService
    {
        Task<string> AuthenticationCallback(string authority, string resource, string scope);
    }
}