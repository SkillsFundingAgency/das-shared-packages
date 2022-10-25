using System.Threading.Tasks;

namespace SFA.DAS.GovUK.Auth.Services
{
    public interface IAzureIdentityService
    {
        Task<string> AuthenticationCallback(string authority, string resource, string scope);
    }
}