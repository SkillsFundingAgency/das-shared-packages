using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.Interfaces
{
    public interface IDfESignInService
    {
        Task PopulateAccountClaims(TokenValidatedContext ctx);
    }
}