using System.Threading.Tasks;

namespace SFA.DAS.Api.Common.Interfaces
{
    public interface IAzureClientCredentialHelper
    {
        Task<string> GetAccessTokenAsync(string identifier);
    }
}