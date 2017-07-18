using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using NLog;

namespace SFA.DAS.NLog.Logger.Web
{
    public class RequestUriMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var key = Constants.RequestUri;
            var value = string.Empty;

            if (request.Headers.Contains(key))
            {
                var uri = request.RequestUri?.AbsoluteUri;
                if (!string.IsNullOrEmpty(uri))
                    value = uri;
            }

            MappedDiagnosticsLogicalContext.Set(key, value);

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}