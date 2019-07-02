using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Http.Logging;

namespace SFA.DAS.Http.MessageHandlers
{
    public class LoggingMessageHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingMessageHandler> _logger;
        
        public LoggingMessageHandler(ILogger<LoggingMessageHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            
            _logger.LogSendingRequest(request.Method, request.RequestUri);
            
            var stopwatch = Stopwatch.StartNew();
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            
            _logger.LogReceivedResponse(stopwatch.ElapsedMilliseconds, response.StatusCode);

            return response;
        }
    }
}