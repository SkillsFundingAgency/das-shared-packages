using SFA.DAS.DfESignIn.Auth.Api.Client;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.Interfaces
{
    public interface IDfESignInClientFactory
    {
        Task<HttpResponseMessage> Request(string userId = "", string organisationId = "");
        void Dispose();
    }
}