using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using NLog;

namespace SFA.DAS.NLog.Logger.Web.MessageHandlers
{
    public class CorrelationIdMessageResponseHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headerName = Constants.HeaderNameRequestCorrelationId;
            var id = $"{Guid.NewGuid()}";

            if (request.Headers.Contains(headerName))
            {
                var values = request.Headers.GetValues(headerName).ToArray();
                if (values.Any())
                    id = values.First();
            }   

            MappedDiagnosticsLogicalContext.Set(headerName, id);

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}