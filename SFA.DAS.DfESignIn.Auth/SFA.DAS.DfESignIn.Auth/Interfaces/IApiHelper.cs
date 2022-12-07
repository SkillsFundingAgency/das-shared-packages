using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.Interfaces
{
    public interface IApiHelper
    {
        string AccessToken { get; set; }

        Task<T> Get<T>(string endpoint);
    }
}
