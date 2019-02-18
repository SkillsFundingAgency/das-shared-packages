using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace SFA.DAS.Http
{
    public class RestHttpClient: IRestHttpClient
    {
        private readonly HttpClient _httpClient;
        
        public RestHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> Get<T>(Uri uri, object queryData = null, CancellationToken cancellationToken = default)
        {
            var response = await GetResponse(uri, queryData, cancellationToken);
            return await response.Content.ReadAsAsync<T>(cancellationToken);
        }
        
        public Task<T> Get<T>(string uri, object queryData = null, CancellationToken cancellationToken = default)
        {
            return Get<T>(new Uri(uri, UriKind.RelativeOrAbsolute), queryData, cancellationToken);
        }

        public async Task<string> Get(Uri uri, object queryData = null, CancellationToken cancellationToken = default)
        {
            var response = await GetResponse(uri, queryData, cancellationToken);
            return await response.Content.ReadAsStringAsync();
        }
        
        public Task<string> Get(string uri, object queryData = null, CancellationToken cancellationToken = default)
        {
            return Get(new Uri(uri, UriKind.RelativeOrAbsolute), queryData, cancellationToken);
        }

        public async Task<string> PostAsJson<TRequest>(string uri, TRequest requestData, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(uri, requestData, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw CreateClientException(response, await response.Content.ReadAsStringAsync());
            }

            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        public async Task<TResponse> PostAsJson<TRequest, TResponse>(string uri, TRequest requestData, CancellationToken cancellationToken = default)
        {
            var resultAsString = await PostAsJson<TRequest>(uri, requestData, cancellationToken);

            var result = JsonConvert.DeserializeObject<TResponse>(resultAsString);

            return result;
        }

        protected virtual Exception CreateClientException(HttpResponseMessage httpResponseMessage, string content)
        {
            return new RestHttpClientException(httpResponseMessage, content);
        }

        protected virtual async Task<HttpResponseMessage> GetResponse(Uri uri, object queryData = null, CancellationToken cancellationToken = default)
        {
            if (queryData != null)
            {
                uri = new Uri(AddQueryString(uri.ToString(), queryData), UriKind.RelativeOrAbsolute);
            }

            var response = await _httpClient.GetAsync(uri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw CreateClientException(response, await response.Content.ReadAsStringAsync());
            }

            return response;
        }
        
        private string AddQueryString(string uri, object queryData)
        {
            var queryDataDictionary = queryData.GetType().GetProperties()
                .ToDictionary(x => x.Name, x => x.GetValue(queryData)?.ToString() ?? string.Empty);
            return QueryHelpers.AddQueryString(uri, queryDataDictionary);
        }
    }
}