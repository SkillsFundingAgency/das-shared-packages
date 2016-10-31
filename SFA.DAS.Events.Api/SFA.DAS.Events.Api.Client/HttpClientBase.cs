using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Events.Api.Client
{
    public abstract class HttpClientBase
    {
        private readonly string _clientToken;

        protected HttpClientBase(string clientToken)
        {
            _clientToken = clientToken;
        }

        protected async Task<string> GetAsync(string url)
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

        protected async Task<string> PostAsync(string url, string data)
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

        protected async Task<string> PutAsync(string url, string data)
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

        protected async Task<string> PatchAsync(string url, string data)
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
    }
}
