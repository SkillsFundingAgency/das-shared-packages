using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Commitments.Api.Client
{
    public abstract class HttpClientBase
    {
        private readonly string _clientToken;

        protected HttpClientBase(string clientToken)
        {
            _clientToken = clientToken;
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
                var queryString = GetQueryString(data);
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

        public string GetQueryString(object obj)
        {
            var result = new List<string>();
            var props = obj.GetType().GetProperties().Where(p => p.GetValue(obj, null) != null);
            foreach (var p in props)
            {
                var value = p.GetValue(obj, null);
                var enumerable = value as ICollection;
                if (enumerable != null)
                {
                    result.AddRange(from object v in enumerable select $"{p.Name}={v}");
                }
                else
                {
                    result.Add($"{p.Name}={value}");
                }
            }

            return string.Join("&", result.ToArray());
        }
    }
}
