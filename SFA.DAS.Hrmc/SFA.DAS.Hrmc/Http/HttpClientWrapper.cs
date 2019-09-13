using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Hrmc.Http
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly IHttpResponseLogger _httpResponseLogger;
        private readonly ILog _logger;

        public HttpClientWrapper(ILog logger, IHttpResponseLogger httpResponseLogger)
        {
            _logger = logger;
            MediaTypeWithQualityHeaderValueList = new List<MediaTypeWithQualityHeaderValue>();
            _httpResponseLogger = httpResponseLogger;
        }

        public string AuthScheme { get; set; }
        public string BaseUrl { get; set; }
        public List<MediaTypeWithQualityHeaderValue> MediaTypeWithQualityHeaderValueList { get; set; }

        public async Task<string> SendMessage<T>(T content, string url)
        {
            using (var httpClient = CreateHttpClient())
            {
                var serializeObject = JsonConvert.SerializeObject(content);
                var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(serializeObject, Encoding.UTF8, "application/json")
                });
                await EnsureSuccessfulResponse(response);

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<T> Get<T>(string authToken, string url)
        {
            using (var httpClient = CreateHttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthScheme, authToken);

                var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
                await EnsureSuccessfulResponse(response);

                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<string> GetString(string url, string accessToken)
        {
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(accessToken))
                {
                    var authScheme = !string.IsNullOrEmpty(AuthScheme)
                        ? AuthScheme
                        : "Bearer";

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authScheme, accessToken);
                }

                var response = await client.GetAsync(url);
                await EnsureSuccessfulResponse(response);

                return response.Content.ReadAsStringAsync().Result;
            }
        }

        private HttpClient CreateHttpClient()
        {
            if (string.IsNullOrEmpty(BaseUrl)) throw new ArgumentNullException(nameof(BaseUrl));

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };

            if (MediaTypeWithQualityHeaderValueList.Any())
                foreach (var mediaTypeWithQualityHeaderValue in MediaTypeWithQualityHeaderValueList)
                    httpClient.DefaultRequestHeaders.Accept.Add(mediaTypeWithQualityHeaderValue);

            return httpClient;
        }

        private async Task EnsureSuccessfulResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;

            switch ((int) response.StatusCode)
            {
                case 404:
                    throw new ResourceNotFoundException(response.RequestMessage.RequestUri.ToString());
                case 408:
                    throw new RequestTimeOutException();
                case 429:
                    throw new TooManyRequestsException();
                case 500:
                    throw new InternalServerErrorException();
                case 503:
                    throw new ServiceUnavailableException();
                default:
                    if ((int) response.StatusCode == 400) await _httpResponseLogger.LogResponseAsync(_logger, response);

                    throw new HttpException((int) response.StatusCode, $"Unexpected HTTP exception - ({(int) response.StatusCode}): {response.ReasonPhrase}");
            }
        }
    }
}