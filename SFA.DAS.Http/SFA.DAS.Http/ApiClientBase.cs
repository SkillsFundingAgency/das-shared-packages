using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace SFA.DAS.Http
{
    public abstract class ApiClientBase
    {
        private readonly QueryStringHelper _queryStringHelper;
        private readonly HttpClient _client;

        protected ApiClientBase(HttpClient client)
        {
            _client = client;
            _queryStringHelper = new QueryStringHelper();
        }

        protected async virtual Task<string> GetAsync(string url)
        {
            string content;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _client.SendAsync(requestMessage);
            content = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();


            return content;
        }

        protected async virtual Task<string> GetAsync(string url, object data)
        {
            string content;

            var queryString = _queryStringHelper.GetQueryString(data);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{url}{queryString}");

            var response = await _client.SendAsync(requestMessage);
            content = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return content;
        }

        protected async virtual Task<string> PostAsync(string url, string data)
        {
            string content;

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(requestMessage);
            content = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return content;
        }

        protected async virtual Task<string> PutAsync(string url, string data)
        {
            string content;

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(requestMessage);
            content = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return content;
        }

        protected async virtual Task<string> PatchAsync(string url, string data)
        {
            string content;

            var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(requestMessage);
            content = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            return content;
        }

        protected async virtual Task DeleteAsync(string url, string data)
        {
            HttpRequestMessage requestMessage;

            if (string.IsNullOrWhiteSpace(data))
            {
                requestMessage = new HttpRequestMessage(HttpMethod.Delete, url);
            }
            else
            {
                requestMessage = new HttpRequestMessage(HttpMethod.Delete, url)
                {
                    Content = new StringContent(data, Encoding.UTF8, "application/json")
                };
            }

            var response = await _client.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
        }
    }
}
