using System.Threading.Tasks;

namespace SFA.DAS.Api.Authentication.Interfaces
{
    public interface IAzureClientCredentialHelper
    {
        Task<string> GetAccessTokenAsync(string identifier);
    }
}