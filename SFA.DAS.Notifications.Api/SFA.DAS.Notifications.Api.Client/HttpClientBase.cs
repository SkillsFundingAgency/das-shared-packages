using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Notifications.Api.Client
{
    public abstract class HttpClientBase
    {
        private readonly string _clientToken;

        protected HttpClientBase(string clientToken)
        {
            _clientToken = clientToken;
        }
        
        protected async Task<string> PostAsync(string url, string data)
        {
            return await GetValue(url, data, new HttpMethod(HttpMethod.Post.ToString()));
        }
        

        private async Task<string> GetValue(string url, string data, HttpMethod method)
        {
            using (var client = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(method, url)
                {
                    Content = new StringContent(data, Encoding.UTF8, "application/json")
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _clientToken);
                var response = await client.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}