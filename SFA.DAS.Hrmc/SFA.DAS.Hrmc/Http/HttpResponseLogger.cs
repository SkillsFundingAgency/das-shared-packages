using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Hrmc.Http
{
    public class HttpResponseLogger : IHttpResponseLogger
    {
        public async Task LogResponseAsync(ILog logger, HttpResponseMessage response)
        {
            if (IsContentStringType(response))
            {
                var content = await response.Content.ReadAsStringAsync();

                logger.Debug("Logged response", new Dictionary<string, object>
                {
                    { "StatusCode", response.StatusCode },
                    { "Reason", response.ReasonPhrase },
                    { "Content", content }
                });
            }
        }

        private bool IsContentStringType(HttpResponseMessage response)
        {
            return response?.Content?.Headers?.ContentType != null && (
                       response.Content.Headers.ContentType.MediaType.StartsWith("text") ||
                       response.Content.Headers.ContentType.MediaType == "application/json");
        }
    }
}