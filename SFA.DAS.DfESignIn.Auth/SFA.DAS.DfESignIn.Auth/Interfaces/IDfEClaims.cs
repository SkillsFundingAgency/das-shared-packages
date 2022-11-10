using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.Interfaces
{
    public interface IDfEClaims
    {
        Task GetClaims(TokenValidatedContext ctx, string userId, string userOrgId, IConfiguration config);
    }
}