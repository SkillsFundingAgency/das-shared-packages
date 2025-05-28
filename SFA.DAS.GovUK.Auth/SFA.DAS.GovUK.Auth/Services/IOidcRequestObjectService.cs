using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.GovUK.Auth.Services
{
    public interface IOidcRequestObjectService
    {
        Task<string> BuildRequestJwtAsync(
            string baseUrl,
            string clientId,
            string responseType,
            string redirectUri,
            string scopes,
            string state,
            string nonce,
            string[] vtr,
            Dictionary<string, object>? claims = null);
    }
}
