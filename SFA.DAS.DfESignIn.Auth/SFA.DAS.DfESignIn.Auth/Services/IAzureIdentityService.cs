using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.Services
{
    public interface IAzureIdentityService
    {
        Task<string> AuthenticationCallback(string authority, string resource, string scope);
    }
}