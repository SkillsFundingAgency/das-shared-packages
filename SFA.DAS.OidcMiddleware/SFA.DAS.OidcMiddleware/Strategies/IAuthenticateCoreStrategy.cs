using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace SFA.DAS.OidcMiddleware.Strategies
{
    public interface IAuthenticateCoreStrategy
    {
        Task<AuthenticationTicket> Authenticate(IOwinContext context);
    }
}