using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Interfaces;
using SFA.DAS.GovUK.Auth.Models;

[assembly: InternalsVisibleTo("SFA.DAS.GovUK.Auth.UnitTests")]
namespace SFA.DAS.GovUK.Auth.Services;

internal class OidcService : IOidcService
{
    private readonly HttpClient _httpClient;
    private readonly IAzureIdentityService _azureIdentityService;
    private readonly IJwtSecurityTokenService _jwtSecurityTokenService;
    private readonly GovUkOidcConfiguration _configuration;

    public OidcService(
        HttpClient httpClient, 
        IAzureIdentityService azureIdentityService,
        IJwtSecurityTokenService jwtSecurityTokenService,
        IOptions<GovUkOidcConfiguration> configuration)
    {
        _httpClient = httpClient;
        _azureIdentityService = azureIdentityService;
        _jwtSecurityTokenService = jwtSecurityTokenService;
        _configuration = configuration.Value;
        _httpClient.BaseAddress = new Uri(configuration.Value.BaseUrl);
    }

    public async Task<Token?> GetToken(OpenIdConnectMessage? openIdConnectMessage)
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
            new ("code",openIdConnectMessage?.Code ?? ""),
            new ("redirect_uri",openIdConnectMessage?.RedirectUri  ?? ""),
            new ("client_assertion_type","urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
            new ("client_assertion", CreateJwtAssertion()),
        });

        httpRequestMessage.Content.Headers.Clear();
        httpRequestMessage.Content.Headers.Add("Content-Type","application/x-www-form-urlencoded");
                            
                        
        var response = await _httpClient.SendAsync(httpRequestMessage);
        var valueString = await response.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<Token>(valueString);

        return content;
    }

    public async Task PopulateAccountClaims(TokenValidatedContext tokenValidatedContext, Func<TokenValidatedContext, Task<List<Claim>>>? populateAdditionalClaims = null)
    {
        if (tokenValidatedContext.TokenEndpointResponse == null || tokenValidatedContext.Principal == null)
        {
            return;
        }
        var accessToken = tokenValidatedContext.TokenEndpointResponse.Parameters["access_token"];

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "/userinfo")
        {
            
            Headers = 
            {
                UserAgent = { new ProductInfoHeaderValue("DfEApprenticeships","1") },
                Authorization = new AuthenticationHeaderValue("Bearer",accessToken)
            }
        };
        var response = await _httpClient.SendAsync(httpRequestMessage);
        var valueString = response.Content.ReadAsStringAsync().Result;
        var content = JsonSerializer.Deserialize<GovUkUser>(valueString);
        if (content?.Email != null)
        {
            tokenValidatedContext.Principal.Identities.First().AddClaim(new Claim(ClaimTypes.Email, content.Email));    
        }

        if (populateAdditionalClaims != null)
        {
            tokenValidatedContext.Principal.Identities.First().AddClaims(await populateAdditionalClaims(tokenValidatedContext));    
        }

    }

    private string CreateJwtAssertion()
    {
        var jti = Guid.NewGuid().ToString();
        var claimsIdentity = new ClaimsIdentity(
            new List<Claim>
            {
                new ("sub",_configuration.ClientId),
                new ("jti", jti)
                                    
            });
        var signingCredentials = new SigningCredentials(
            new KeyVaultSecurityKey(_configuration.KeyVaultIdentifier, _azureIdentityService.AuthenticationCallback), "RS512")
        {
            CryptoProviderFactory = new CryptoProviderFactory
            {
                CustomCryptoProvider = new KeyVaultCryptoProvider()
            }
        };
        var value = _jwtSecurityTokenService.CreateToken(_configuration.ClientId,
            $"{_configuration.BaseUrl}/token", claimsIdentity, signingCredentials);
        
        return value;
    }
}