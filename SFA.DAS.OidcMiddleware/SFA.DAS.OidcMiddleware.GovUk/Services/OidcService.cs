using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.OidcMiddleware.GovUk.Configuration;
using SFA.DAS.OidcMiddleware.GovUk.Models;

namespace SFA.DAS.OidcMiddleware.GovUk.Services
{
    public class OidcService : IOidcService
    {
        private readonly HttpClient _httpClient;
        private readonly IAzureIdentityService _azureIdentityService;
        private readonly IJwtSecurityTokenService _jwtSecurityTokenService;
        private readonly GovUkOidcConfiguration _configuration;

        public OidcService(
            HttpClient httpClient,
            IAzureIdentityService azureIdentityService,
            IJwtSecurityTokenService jwtSecurityTokenService,
            GovUkOidcConfiguration configuration)
        {
            _httpClient = httpClient;
            _azureIdentityService = azureIdentityService;
            _jwtSecurityTokenService = jwtSecurityTokenService;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(configuration.BaseUrl);
        }

        public Token GetToken(string code, string redirectUri)
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
                new KeyValuePair<string, string> ("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code ?? ""),
                new KeyValuePair<string, string>("redirect_uri", redirectUri ?? ""),
                new KeyValuePair<string, string>("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
                new KeyValuePair<string, string>("client_assertion", CreateJwtAssertion()),
            });

            httpRequestMessage.Content.Headers.Clear();
            httpRequestMessage.Content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");


            var response = _httpClient.SendAsync(httpRequestMessage).Result;
            var valueString = response.Content.ReadAsStringAsync().Result;
            var content = JsonSerializer.Deserialize<Token>(valueString);

            return content;
        }

        public void PopulateAccountClaims(ClaimsIdentity claimsIdentity, string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken) || claimsIdentity == null)
            {
                return;
            }

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "/userinfo")
            {

                Headers =
                {
                    UserAgent = {new ProductInfoHeaderValue("DfEApprenticeships", "1")},
                    Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
                }
            };
            var response = _httpClient.SendAsync(httpRequestMessage).Result;
            var valueString = response.Content.ReadAsStringAsync().Result;
            var content = JsonSerializer.Deserialize<GovUkUser>(valueString);
            if (content?.Email != null)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, content.Email));
            }
        }

        private string CreateJwtAssertion()
        {
            var jti = Guid.NewGuid().ToString();
            var claimsIdentity = new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim ("sub", _configuration.ClientId),
                    new Claim ("jti", jti)

                });
            var signingCredentials = new SigningCredentials(
                new KeyVaultSecurityKey(_configuration.KeyVaultIdentifier,
                    _azureIdentityService.AuthenticationCallback), "RS512")
            {
                CryptoProviderFactory = new CryptoProviderFactory
                {
                    CustomCryptoProvider = new KeyVaultCryptoProvider()
                }
            };
            var value = _jwtSecurityTokenService.CreateToken(claimsIdentity, signingCredentials);

            return value;
        }

    }
}