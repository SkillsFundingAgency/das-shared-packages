using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using NLog;

namespace SFA.DAS.NLog.Logger.Web.MessageHandlers
{
    /// <summary>
    /// DelegatingHandler that is adding sesssion id from NLog context to request header
    /// </summary>
    public class SessionIdMessageRequestHandler : DelegatingHandler
    {
        public SessionIdMessageRequestHandler() { }

        public SessionIdMessageRequestHandler(HttpMessageHandler innerHandler) : base(innerHandler) {}

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var id = MappedDiagnosticsLogicalContext.Get(Constants.HeaderNameSessionCorrelationId);
            request.Headers.Add(Constants.HeaderNameSessionCorrelationId, id);

            return base.SendAsync(request, cancellationToken);
        }
    }
}