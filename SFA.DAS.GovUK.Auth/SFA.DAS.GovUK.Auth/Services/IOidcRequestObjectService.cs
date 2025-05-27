using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.GovUK.Auth.Services
{
    public interface IOidcRequestObjectService
    {
        Task<string> BuildRequestJwtAsync(
            string baseUrl,
            string clientId,
            string redirectUri,
            string scopes,
            string state,
            string nonce,
            string[] vtr,
            Dictionary<string, object>? claims = null);
    }
}
