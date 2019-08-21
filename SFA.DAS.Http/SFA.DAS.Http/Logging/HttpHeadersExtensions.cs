using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace SFA.DAS.Http.Logging
{
    internal static class HttpHeadersExtensions
    {
        public static string ToLogMessage(this HttpRequestHeaders httpRequestHeaders, HttpContentHeaders httpContentHeaders)
        {
            return GetLogMessage(httpRequestHeaders, httpContentHeaders);
        }
        
        public static string ToLogMessage(this HttpResponseHeaders httpResponseHeaders, HttpContentHeaders httpContentHeaders)
        {
            return GetLogMessage(httpResponseHeaders, httpContentHeaders);
        }
        
        private static string GetLogMessage(HttpHeaders httpHeaders, HttpContentHeaders httpContentHeaders)
        {
            var allHttpHeaders = httpContentHeaders == null ? httpHeaders : httpHeaders.Concat(httpContentHeaders);

            var message = allHttpHeaders
                .Aggregate(new StringBuilder(), (b, h) => b
                    .AppendLine()
                    .Append(h.Key)
                    .Append(": ")
                    .AppendJoin(", ", h.Value))
                .ToString();
            
            return message;
        }
    }
}