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
            var response = await GetResponse(uri, queryData, cancellationToken).ConfigureAwait(false);
            return await response.Content.ReadAsAsync<T>(cancellationToken).ConfigureAwait(false);
        }
        
        public Task<T> Get<T>(string uri, object queryData = null, CancellationToken cancellationToken = default)
        {
            return Get<T>(new Uri(uri, UriKind.RelativeOrAbsolute), queryData, cancellationToken);
        }

        public async Task<string> Get(Uri uri, object queryData = null, CancellationToken cancellationToken = default)
        {
            var response = await GetResponse(uri, queryData, cancellationToken).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        
        public Task<string> Get(string uri, object queryData = null, CancellationToken cancellationToken = default)
        {
            return Get(new Uri(uri, UriKind.RelativeOrAbsolute), queryData, cancellationToken);
        }

        public async Task<TResponse> PostAsJson<TResponse>(string uri, CancellationToken cancellationToken = default)
        {
            var resultAsString = await PostAsJson<object>(uri, null, cancellationToken).ConfigureAwait(false);

            var result = JsonConvert.DeserializeObject<TResponse>(resultAsString);

            return result;
        }

        public async Task<string> PostAsJson<TRequest>(string uri, TRequest requestData, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(uri, requestData, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw CreateClientException(response, await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return result;
        }

        public async Task<TResponse> PostAsJson<TRequest, TResponse>(string uri, TRequest requestData, CancellationToken cancellationToken = default)
        {
            var resultAsString = await PostAsJson(uri, requestData, cancellationToken).ConfigureAwait(false);

            var result = JsonConvert.DeserializeObject<TResponse>(resultAsString);

            return result;
        }

        public async Task<string> PutAsJson<TRequest>(string uri, TRequest requestData, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync<TRequest>(uri, requestData, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw CreateClientException(response, await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return result;
        }

        public async Task<TResponse> PutAsJson<TRequest, TResponse>(string uri, TRequest requestData, CancellationToken cancellationToken = default)
        {
            var resultAsString = await PutAsJson(uri, requestData, cancellationToken).ConfigureAwait(false);

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

            var response = await _httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw CreateClientException(response, await response.Content.ReadAsStringAsync().ConfigureAwait(false));
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