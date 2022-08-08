using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Interfaces;
using SFA.DAS.GovUK.Auth.Models;

[assembly: InternalsVisibleTo("SFA.DAS.GovUK.Auth.UnitTests")]
namespace SFA.DAS.GovUK.Auth.Services;

internal class OidcService : IOidcService
{
    private readonly HttpClient _httpClient;

    public OidcService(
        HttpClient httpClient, 
        IOptions<GovUkOidcConfiguration> configuration)
    {
        _httpClient = httpClient;

        if (configuration.Value.BaseUrl == null)
        {
            throw new ArgumentException("Value cannot be null. (Parameter 'configuration.Value.BaseUrl')", nameof(configuration.Value.BaseUrl));
        }
        
        _httpClient.BaseAddress = new Uri(configuration.Value.BaseUrl);
    }

    public async Task<Token?> GetToken(OpenIdConnectMessage openIdConnectMessage, string clientAssertion)
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/token")
        {
            Headers = 
            {
                Accept = { new MediaTypeWithQualityHeaderValue("*/*"), new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded") },
                UserAgent = { new ProductInfoHeaderValue("DfEApprenticeships","1") },
            }
        };
        
        httpRequestMessage.Content = new FormUrlEncodedContent (new List<KeyValuePair<string, string>>
        {
            new ("grant_type","authorization_code"),
            new ("code",openIdConnectMessage.Code),
            new ("redirect_uri",openIdConnectMessage.RedirectUri),
            new ("client_assertion_type","urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
            new ("client_assertion",clientAssertion),
        });

        httpRequestMessage.Content.Headers.Clear();
        httpRequestMessage.Content.Headers.Add("Content-Type","application/x-www-form-urlencoded");
                            
                        
        var response = await _httpClient.SendAsync(httpRequestMessage);
        var valueString = await response.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<Token>(valueString);

        return content;
    }
}