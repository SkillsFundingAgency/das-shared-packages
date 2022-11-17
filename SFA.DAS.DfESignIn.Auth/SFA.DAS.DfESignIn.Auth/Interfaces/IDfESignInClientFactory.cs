using SFA.DAS.DfESignIn.Auth.Api.Client;

namespace SFA.DAS.DfESignIn.Auth.Interfaces
{
    public interface IDfESignInClientFactory
    {
        DfESignInClient CreateDfESignInClient(string userId = "", string organisationId = "");
        void Dispose();
    }
}