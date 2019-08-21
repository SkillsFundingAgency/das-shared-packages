using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Http.Logging
{
    internal static class LoggerExtensions
    {
        private static readonly Action<ILogger, HttpMethod, Uri, Exception> LogSendingRequest = LoggerMessage.Define<HttpMethod, Uri>(
            LogLevel.Information,
            EventIds.SendingRequest,
            "Sending HTTP request {HttpMethod} {Uri}");
        
        private static readonly Action<ILogger, string, Exception> LogSendingRequestHeaders = LoggerMessage.Define<string>(
            LogLevel.Trace,
            EventIds.SendingRequestHeaders,
            "Sending request headers: {Headers}");
        
        private static readonly Action<ILogger, double, HttpStatusCode, Exception> LogReceivedResponse = LoggerMessage.Define<double, HttpStatusCode>(
            LogLevel.Information,
            EventIds.ReceivedResponse,
            "Received HTTP response after {ElapsedMilliseconds}ms - {HttpStatusCode}");
        
        private static readonly Action<ILogger, string, Exception> LogReceivedResponseHeaders = LoggerMessage.Define<string>(
            LogLevel.Trace,
            EventIds.ReceivedResponseHeaders,
            "Received response headers: {Headers}");
        
        public static void LogRequest(this ILogger logger, HttpRequestMessage request)
        {
            LogSendingRequest(logger, request.Method, request.RequestUri, null);

            if (logger.IsEnabled(LogLevel.Trace))
            {
                LogSendingRequestHeaders(logger, request.Headers.ToLogMessage(request.Content?.Headers), null);
            }
        }

        public static void LogResponse(this ILogger logger, HttpResponseMessage response, long elapsedMilliseconds)
        {
            LogReceivedResponse(logger, elapsedMilliseconds, response.StatusCode, null);
            
            if (logger.IsEnabled(LogLevel.Trace))
            {
                LogReceivedResponseHeaders(logger, response.Headers.ToLogMessage(response.Content?.Headers), null);
            }
        }
    }
}