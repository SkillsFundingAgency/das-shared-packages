using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Tasks.Api.Client
{
    public abstract class HttpClientBase
    {
        protected async Task<string> GetAsync(string url)
        {
            var content = "";

            try
            {
                using (var client = new HttpClient())
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                    // Add custom headers
                    //requestMessage.Headers.Add("User-Agent", "User-Agent-Here");

                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("WRAP", "bigAccessToken");
                    var response = await client.SendAsync(requestMessage);
                    content = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (HttpRequestException ex)
            {
                throw;
            }

            return content;
        }

        protected async Task<string> PostAsync(string url, string data)
        {
            var content = "";

            try
            {
                using (var client = new HttpClient())
                {

                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new StringContent(data, Encoding.UTF8, "application/json")
                    };

                    // Add custom headers
                    //requestMessage.Headers.Add("User-Agent", "User-Agent-Here");

                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("WRAP", "bigAccessToken");
                    var response = await client.SendAsync(requestMessage);
                    content = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (HttpRequestException ex)
            {
                throw;
            }

            return content;
        }

        protected async Task<string> PutAsync(string url, string data)
        {
            var content = "";

            try
            {
                using (var client = new HttpClient())
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Put, url)
                    {
                        Content = new StringContent(data, Encoding.UTF8, "application/json")
                    };

                    // Add custom headers
                    //requestMessage.Headers.Add("User-Agent", "User-Agent-Here");

                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("WRAP", "bigAccessToken");
                    var response = await client.SendAsync(requestMessage);
                    content = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (HttpRequestException ex)
            {
                throw;
            }

            return content;
        }

        protected async Task<string> PatchAsync(string url, string data)
        {
            var content = "";

            try
            {
                using (var client = new HttpClient())
                {
                    var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), url)
                    {
                        Content = new StringContent(data, Encoding.UTF8, "application/json")
                    };

                    // Add custom headers
                    //requestMessage.Headers.Add("User-Agent", "User-Agent-Here");

                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("WRAP", "bigAccessToken");
                    var response = await client.SendAsync(requestMessage);
                    content = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (HttpRequestException ex)
            {
                throw;
            }

            return content;
        }

    }
}