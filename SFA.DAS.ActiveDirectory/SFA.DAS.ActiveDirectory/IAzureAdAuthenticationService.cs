using System.Threading.Tasks;

namespace SFA.DAS.ActiveDirectory
{
    public interface IAzureAdAuthenticationService
    {
        Task<string> GetAuthenticationResult(string clientId, string appKey, string resourceId, string tenant);
    }
}
