using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace SFA.DAS.DfESignIn.Auth.Interfaces
{
    public interface IDfESignInService
    {
        Task PopulateAccountClaims(TokenValidatedContext ctx);
    }
}