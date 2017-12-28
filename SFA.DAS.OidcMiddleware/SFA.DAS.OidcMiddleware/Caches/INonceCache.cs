using System.Threading.Tasks;
using Microsoft.Owin.Security;

namespace SFA.DAS.OidcMiddleware.Caches
{
    public interface INonceCache
    {
        Task<string> GetNonceAsync(IAuthenticationManager authenticationManager);
        Task SetNonceAsync(IAuthenticationManager authenticationManager, string nonce);
    }
}