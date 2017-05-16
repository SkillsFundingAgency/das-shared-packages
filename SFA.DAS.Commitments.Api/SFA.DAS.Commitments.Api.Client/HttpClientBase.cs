using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Commitments.Api.Client
{
    public abstract class HttpClientBase
    {
        private readonly string _clientToken;

        private readonly QueryStringHelper _queryStringHelper;

        protected HttpClientBase(string clientToken)
        {
            _clientToken = clientToken;
            _queryStringHelper = new QueryStringHelper();
        }

        public async Task<string> GetAsync(string url)
        {
            string content;

            using (var client = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _clientToken);
                var response = await client.SendAsync(requestMessage);
                content = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }

            return content;
        }

        public async Task<string> GetAsync(string url, object data)
        {
            string content;

            using (var client = new HttpClient())
            {
                var queryString = _queryStringHelper.GetQueryString(data);
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{url}{queryString}");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _clientToken);
                var response = await client.SendAsync(requestMessage);
                content = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }

            return content;
        }

        public async Task<string> PostAsync(string url, string data)
        {
            string content;

            using (var client = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(data, Encoding.UTF8, "application/json")
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _clientToken);
                var response = await client.SendAsync(requestMessage);
                content = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }

            return content;
        }

        public async Task<string> PutAsync(string url, string data)
        {
            string content;

            using (var client = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Put, url)
                {
                    Content = new StringContent(data, Encoding.UTF8, "application/json")
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _clientToken);
                var response = await client.SendAsync(requestMessage);
                content = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }

            return content;
        }

        public async Task<string> PatchAsync(string url, string data)
        {
            string content;

            using (var client = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), url)
                {
                    Content = new StringContent(data, Encoding.UTF8, "application/json")
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _clientToken);
                var response = await client.SendAsync(requestMessage);
                content = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }

            return content;
        }

        public async Task DeleteAsync(string url, string data)
        {
            using (var client = new HttpClient())
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

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _clientToken);
                var response = await client.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
