using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System;

namespace SFA.DAS.Authentication.Extensions.Legacy
{
    // "Included to allow older clients to be updated to use AAD without upgrading those clients away from using ApiClientBase"
    public abstract class ApiClientBase
    {
        private readonly QueryStringHelper _queryStringHelper;

        private readonly HttpClient _client;

        protected ApiClientBase(HttpClient client)
        {
            _client = client;
            _queryStringHelper = new QueryStringHelper();
        }

        protected virtual async Task<string> GetAsync(string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = await _client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return result;
        }

        protected virtual async Task<string> GetAsync(string url, object data)
        {
            string queryString = _queryStringHelper.GetQueryString(data);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{url}{queryString}");
            HttpResponseMessage response = await _client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return result;
        }

        protected virtual async Task<string> PostAsync(string url, string data)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };
            HttpResponseMessage response = await _client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return result;
        }

        protected virtual async Task<string> PutAsync(string url, string data)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };
            HttpResponseMessage response = await _client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return result;
        }

        protected virtual async Task<string> PatchAsync(string url, string data)
        {
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };
            HttpResponseMessage response = await _client.SendAsync(request);
            string result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return result;
        }

        protected virtual async Task DeleteAsync(string url, string data)
        {
            HttpRequestMessage request = (!string.IsNullOrWhiteSpace(data)) ? new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            } : new HttpRequestMessage(HttpMethod.Delete, url);
            (await _client.SendAsync(request)).EnsureSuccessStatusCode();
        }
    }
}
