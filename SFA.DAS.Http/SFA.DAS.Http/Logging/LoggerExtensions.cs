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
        
        private static readonly Action<ILogger, double, HttpStatusCode, Exception> ReceivedResponse = LoggerMessage.Define<double, HttpStatusCode>(
            LogLevel.Information,
            EventIds.ReceivedResponse,
            "Received HTTP response after {ElapsedMilliseconds}ms - {HttpStatusCode}");
        
        public static void LogSendingRequest(this ILogger logger, HttpMethod httpMethod, Uri uri)
        {
            SendingRequest(logger, httpMethod, uri, null);
        }

        public static void LogReceivedResponse(this ILogger logger, double elapsedMilliseconds, HttpStatusCode httpStatusCode)
        {
            ReceivedResponse(logger, elapsedMilliseconds, httpStatusCode, null);
        }
    }
}