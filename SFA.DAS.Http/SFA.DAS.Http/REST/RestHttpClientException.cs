using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Http.REST
{
    public class RestHttpClientException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ReasonPhrase { get; }
        public Uri RequestUri { get; }
        public string ErrorResponse { get; }
    
        public RestHttpClientException(HttpResponseMessage httpResponseMessage, string errorResponse)
            : base(GenerateMessage(httpResponseMessage, errorResponse))
        {
            StatusCode = httpResponseMessage.StatusCode;
            ReasonPhrase = httpResponseMessage.ReasonPhrase;
            RequestUri = httpResponseMessage.RequestMessage.RequestUri;
            ErrorResponse = errorResponse;
        }
            
        // assumes response content hasn't already been read
        public static async Task<RestHttpClientException> Create(HttpResponseMessage httpResponseMessage)
        {
            return new RestHttpClientException(httpResponseMessage, await httpResponseMessage.Content.ReadAsStringAsync());
        }
    
        private static string GenerateMessage(HttpResponseMessage httpResponseMessage, string errorResponse)
        {
            return $@"Request '{httpResponseMessage.RequestMessage.RequestUri}' 
                    returned {(int)httpResponseMessage.StatusCode} {httpResponseMessage.ReasonPhrase}
                    Response: {errorResponse}";
        }
    }
}