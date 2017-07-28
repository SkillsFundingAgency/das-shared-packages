using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using NLog;

namespace SFA.DAS.NLog.Logger.Web.DelegatingHandler
{
    public class CorrelationIdDelegatingHandler : System.Net.Http.DelegatingHandler
    {
        protected CorrelationIdDelegatingHandler(HttpMessageHandler innerHandler) : base(innerHandler) {}

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var id = MappedDiagnosticsLogicalContext.Get(Constants.RequestCorrelationId);
            request.Headers.Add(Constants.RequestCorrelationId, id);

            return base.SendAsync(request, cancellationToken);
        }
    }
}