using System.Net;
using System.Net.Http;

namespace SFA.DAS.SharedOuterApi.Infrastructure
{
    public class HttpRequestContentException : HttpRequestException
    {
        public string? ErrorContent { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public HttpRequestContentException(string message, HttpStatusCode statusCode) : this(message, statusCode, "")
        {
        }

        public HttpRequestContentException(string message, HttpStatusCode statusCode, string? errorContent) : base(message)
        {
            StatusCode = statusCode;
            ErrorContent = errorContent;
        }
    }
}