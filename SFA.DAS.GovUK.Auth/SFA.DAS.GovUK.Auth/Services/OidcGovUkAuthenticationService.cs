using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Models;

[assembly: InternalsVisibleTo("SFA.DAS.GovUK.Auth.UnitTests")]

namespace SFA.DAS.GovUK.Auth.Services;

internal class OidcGovUkAuthenticationService : IGovUkAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ISigningCredentialsProvider _signingProvider;
    private readonly IJwtSecurityTokenService _jwtSecurityTokenService;
    private readonly ICustomClaims _customClaims;
    private readonly GovUkOidcConfiguration _configuration;

    public OidcGovUkAuthenticationService(HttpClient httpClient,
                       ISigningCredentialsProvider signingProvider,
                       IJwtSecurityTokenService jwtSecurityTokenService,
                       GovUkOidcConfiguration configuration,
                       ICustomClaims customClaims)
    {
        _httpClient = httpClient;
        _signingProvider = signingProvider;
        _jwtSecurityTokenService = jwtSecurityTokenService;
        _customClaims = customClaims;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(configuration.BaseUrl);
    }

    public async Task<Token> GetToken(OpenIdConnectMessage openIdConnectMessage)
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/token")
        {
            Headers =
            {
                Accept =
                {
                    new MediaTypeWithQualityHeaderValue("*/*"),
                    new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded")
                },
                UserAgent = {new ProductInfoHeaderValue("DfEApprenticeships", "1")},
            }
        };

        httpRequestMessage.Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
        {
            new("grant_type", "authorization_code"),
            new("code", openIdConnectMessage?.Code ?? ""),
            new("redirect_uri", openIdConnectMessage?.RedirectUri ?? ""),
            new("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
            new("client_assertion", CreateJwtAssertion()),
            new("code_verifier", openIdConnectMessage != null && openIdConnectMessage.Parameters.TryGetValue("code_verifier", out var codeVerifier) ? codeVerifier : "" )
        });

        httpRequestMessage.Content.Headers.Clear();
        httpRequestMessage.Content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");


        var response = await _httpClient.SendAsync(httpRequestMessage);
        var valueString = await response.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<Token>(valueString);

        return content;
    }

    public async Task PopulateAccountClaims(TokenValidatedContext tokenValidatedContext)
    {
        if (tokenValidatedContext.TokenEndpointResponse == null || tokenValidatedContext.Principal == null)
        {
            return;
        }

        var accessToken = tokenValidatedContext.TokenEndpointResponse.Parameters["access_token"];

        var content = await GetAccountDetails(accessToken);

        if (content?.Email != null)
        {
            tokenValidatedContext.Principal.Identities.First().AddClaim(new Claim(ClaimTypes.Email, content.Email));
        }

        tokenValidatedContext.Principal.Identities.First()
            .AddClaims(await _customClaims.GetClaims(tokenValidatedContext));
    }

    public async Task<GovUkUser> GetAccountDetails(string accessToken)
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "/userinfo")
        {
            Headers =
            {
                UserAgent = {new ProductInfoHeaderValue("DfEApprenticeships", "1")},
                Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
            }
        };

        var response = await _httpClient.SendAsync(httpRequestMessage);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var valueString = response.Content.ReadAsStringAsync().Result;
        return JsonSerializer.Deserialize<GovUkUser>(valueString);
    }

    private string CreateJwtAssertion()
    {
        var jti = Guid.NewGuid().ToString();
        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim("sub", _configuration.ClientId),
            new Claim("jti", jti)
        ]);

        var signingCredentials = _signingProvider.GetSigningCredentials();

        var value = _jwtSecurityTokenService.CreateToken(_configuration.ClientId,
            $"{_configuration.BaseUrl}/token", claimsIdentity, signingCredentials);

        return value;
    }
}