using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;

namespace SFA.DAS.OidcMiddleware.Caches
{
    public class NonceCache : INonceCache
    {
        public async Task<string> GetNonceAsync(IAuthenticationManager authenticationManager)
        {
            var data = await authenticationManager.AuthenticateAsync("TempState");

            if (data == null)
            {
                return null;
            }

            var nonce = data.Identity.FindFirst("nonce").Value;

            authenticationManager.SignOut("TempState");

            return nonce;
        }

        public Task SetNonceAsync(IAuthenticationManager authenticationManager, string nonce)
        {
            var identity = new ClaimsIdentity("TempState");

            identity.AddClaim(new Claim("nonce", nonce));

            authenticationManager.SignIn(identity);

            return Task.FromResult<object>(null);
        }
    }
}