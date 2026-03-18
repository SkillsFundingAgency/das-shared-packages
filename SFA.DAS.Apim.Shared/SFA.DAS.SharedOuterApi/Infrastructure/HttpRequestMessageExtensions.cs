using System.Net.Http;

namespace SFA.DAS.SharedOuterApi.Infrastructure
{
    public static class HttpRequestMessageExtensions
    {
        public static void AddVersion(this HttpRequestMessage httpRequestMessage, string version)
        {
            httpRequestMessage.Headers.Add("X-Version", version);
        }

        /// <summary>
        /// Conditionally adds a correlation ID to the request headers if CorrelationIdMiddleware has been added to the projects
        /// start up
        /// </summary>
        /// <remarks>
        /// Add correlation middleware by including this line in your program.cs:
        /// app.UseMiddleware<CorrelationIdMiddleware>();
        /// </remarks>
        public static void AddCorrelationId(this HttpRequestMessage httpRequestMessage)
        {
            var correlationId = CorrelationContext.CorrelationId;

            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                httpRequestMessage.Headers.Add("x-correlation-id", correlationId);
            }
        }
    }
}