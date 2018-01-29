using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers
{
    public class ApiMessageHandlers : DelegateHandler
    {
        private class Response
        {
            public HttpStatusCode _statusCode;
            public object _response;

            public Response(HttpStatusCode statusCode, object response)
            {
                _statusCode = statusCode;
                _response = response;
            }
        }

        private Dictionary<string, Response> _configuredGets = new Dictionary<string, Response>();
        
        public void SetupGet<T>(string endPoint, T response)
        {
            SetupGet<T>(endPoint, HttpStatusCode.OK, response);
        }

        public void SetupGet<T>(string endPoint, HttpStatusCode httpStatusCode, T response)
        {
            _configuredGets.Add(GetEndPoint(endPoint), new Response(httpStatusCode, response));
        }

        public void SetupPut(string url)
        {
            _configuredGets.Remove(url);
        }
        public void ClearSetup()
        {
            _configuredGets.Clear();
        }

        protected override Task<HttpResponseMessage> DoSendAsync(HttpRequestMessage request)
        {
            var requestUri = request.RequestUri.ToString();
            HttpResponseMessage response;
            if (!_configuredGets.ContainsKey(requestUri))
            {
                response = request.CreateResponse(HttpStatusCode.NotFound);
            }
            else
            {
                response = request.CreateResponse(_configuredGets[requestUri]._statusCode, _configuredGets[requestUri]._response);
            }

            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);
            return tsc.Task;
        }

        private string GetEndPoint(string url)
        {
            return url.StartsWith("/")
                ? url.TrimStart('/')
                : url;
        }
    }
}
