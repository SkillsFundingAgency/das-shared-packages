using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging.Syndication.Http
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _client;

        public HttpClientWrapper(string baseServerUrl, IDictionary<string, string[]> defaultHeaders = null)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(baseServerUrl);
            if (defaultHeaders != null)
            {
                AddHeaders(defaultHeaders, _client.DefaultRequestHeaders);
            }
        }
        public async Task<string> Get(string resourceUri, IDictionary<string, string[]> headers = null)
        {
            var response = await _client.SendAsync(MakeRequest("GET", resourceUri, headers));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }


        private HttpRequestMessage MakeRequest(string verb, string url, IDictionary<string, string[]> headers)
        {
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod(verb),
                RequestUri = new Uri(url)
            };
            if (headers != null)
            {
                AddHeaders(headers, request.Headers);
            }
            return request;
        }
        private void AddHeaders(IDictionary<string, string[]> headers, HttpRequestHeaders requestHeaders)
        {
            foreach (var key in headers.Keys)
            {
                requestHeaders.Add(key, headers[key]);
            }
        }
    }
}
