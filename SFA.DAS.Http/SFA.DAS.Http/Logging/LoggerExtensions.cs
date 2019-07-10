using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Http.Logging
{
    internal static class LoggerExtensions
    {
        private static readonly Action<ILogger, HttpMethod, Uri, Exception> SendingRequest = LoggerMessage.Define<HttpMethod, Uri>(
            LogLevel.Information,
            EventIds.SendingRequest,
            "Sending HTTP request {HttpMethod} {Uri}");
        
        private static readonly Action<ILogger, string, Exception> SendingRequestHeaders = LoggerMessage.Define<string>(
            LogLevel.Trace,
            EventIds.SendingRequestHeaders,
            "Sending Request Headers: {Headers}");
        
        private static readonly Action<ILogger, double, HttpStatusCode, Exception> ReceivedResponse = LoggerMessage.Define<double, HttpStatusCode>(
            LogLevel.Information,
            EventIds.ReceivedResponse,
            "Received HTTP response after {ElapsedMilliseconds}ms - {HttpStatusCode}");
        
        private static readonly Action<ILogger, string, Exception> ReceivedResponseHeaders = LoggerMessage.Define<string>(
            LogLevel.Trace,
            EventIds.ReceivedResponseHeaders,
            "Received Response Headers: {Headers}");
        
        public static void LogSendingRequest(this ILogger logger, HttpRequestMessage request)
        {
            SendingRequest(logger, request.Method, request.RequestUri, null);

            if (logger.IsEnabled(LogLevel.Trace))
            {
                SendingRequestHeaders(logger, request.Headers.ToLogMessage(request.Content?.Headers), null);
            }
        }

        public static void LogReceivedResponse(this ILogger logger, HttpResponseMessage response, long elapsedMilliseconds)
        {
            ReceivedResponse(logger, elapsedMilliseconds, response.StatusCode, null);
            
            if (logger.IsEnabled(LogLevel.Trace))
            {
                ReceivedResponseHeaders(logger, response.Headers.ToLogMessage(response.Content?.Headers), null);
            }
        }
    }
}