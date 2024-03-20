using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using NLog;

namespace SFA.DAS.NLog.Logger.Web.MessageHandlers
{
    /// <summary>
    /// DelegatingHandler that is adding (web app) request id from NLog context to request header
    /// </summary>
    public class RequestIdMessageRequestHandler : DelegatingHandler
    {
        public RequestIdMessageRequestHandler()
        {
        }

        public RequestIdMessageRequestHandler(HttpMessageHandler innerHandler) : base(innerHandler) {}

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var id = MappedDiagnosticsLogicalContext.Get(Constants.HeaderNameRequestCorrelationId);
            request.Headers.Add(Constants.HeaderNameRequestCorrelationId, id);

            return base.SendAsync(request, cancellationToken);
        }
    }
}