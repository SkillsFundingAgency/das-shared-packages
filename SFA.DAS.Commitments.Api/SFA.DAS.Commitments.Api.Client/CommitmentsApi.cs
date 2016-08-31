using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.Commitments.Api.Client
{
    public class CommitmentsApi : ICommitmentsApi
    {
        private readonly string _baseUrl;

        public CommitmentsApi(string baseUrl)
        {
            if (baseUrl == null)
                throw new ArgumentNullException(nameof(baseUrl));
            _baseUrl = baseUrl;
        }

        public async Task<List<CommitmentListItem>> GetForProvider(long providerId)
        {
            var url = $"{_baseUrl}api/provider/{providerId}/commitments";

            var content = await Execute(url);

            return JsonConvert.DeserializeObject<List<CommitmentListItem>>(content);
        }

        private async Task<string> Execute(string url)
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
    }
}