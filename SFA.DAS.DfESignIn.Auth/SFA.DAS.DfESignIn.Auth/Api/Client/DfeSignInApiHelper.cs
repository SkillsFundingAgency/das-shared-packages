using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.Interfaces;

namespace SFA.DAS.DfESignIn.Auth.Api.Client
{
    public class DfeSignInApiHelper : IApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenBuilder _tokenBuilder;


        public DfeSignInApiHelper(HttpClient httpClient, ITokenBuilder tokenBuilder)
        {
            _httpClient = httpClient;
            _tokenBuilder = tokenBuilder;
        }

        public async Task<T> Get<T>(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Authorization =  new AuthenticationHeaderValue("Bearer", _tokenBuilder.CreateToken());
            var getResponse = await _httpClient.SendAsync(request);
            return getResponse.IsSuccessStatusCode ? JsonConvert.DeserializeObject<T>(await getResponse.Content.ReadAsStringAsync()) : default;
        }
    }
}
