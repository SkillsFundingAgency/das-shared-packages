using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.Api.Client
{
    public class DfeSignInApiHelper : IApiHelper, IDisposable
    {
        private readonly HttpClient _httpClient;
        public string AccessToken { get; set; }

        public DfeSignInApiHelper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Method to make GET call for a HttpClient.
        /// </summary>
        /// <typeparam name="T">TResult.</typeparam>
        /// <param name="endpoint">Destination endpoint.</param>
        /// <returns>TResult.</returns>
        /// <exception cref="MemberAccessException"></exception>
        public async Task<T> Get<T>(string endpoint)
        {
            #region Check Arguments & Members
            if (string.IsNullOrEmpty(endpoint)) throw new ArgumentNullException(nameof(endpoint));
            if (string.IsNullOrEmpty(AccessToken)) throw new MemberAccessException(nameof(AccessToken));
            #endregion

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            var getResponse = await _httpClient.GetAsync(endpoint);
            return getResponse.IsSuccessStatusCode ? JsonConvert.DeserializeObject<T>(await getResponse.Content.ReadAsStringAsync()) : default;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
