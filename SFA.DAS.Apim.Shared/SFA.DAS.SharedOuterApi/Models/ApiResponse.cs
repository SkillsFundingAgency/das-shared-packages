using System.Collections.Generic;
using System.Net;

namespace SFA.DAS.SharedOuterApi.Models
{
    public class ApiResponse<TResponse>
    {
        public TResponse Body { get;  }
        public HttpStatusCode StatusCode { get; }
        public string ErrorContent { get ; }
        public Dictionary<string, IEnumerable<string>> Headers { get; }
        public string RawContent { get; set; } // Always holds the unmodified body

        public ApiResponse(TResponse body, HttpStatusCode statusCode, string errorContent) :this(body, statusCode, errorContent, new Dictionary<string, IEnumerable<string>>())
        {
            
        }

        public ApiResponse (TResponse body, HttpStatusCode statusCode, string errorContent, Dictionary<string, IEnumerable<string>> headers)
        {
            Body = body;
            StatusCode = statusCode;
            ErrorContent = errorContent;
            Headers = headers;
        }
    }
}