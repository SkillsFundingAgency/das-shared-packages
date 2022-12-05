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
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Models;

[assembly: InternalsVisibleTo("SFA.DAS.GovUK.Auth.UnitTests")]

namespace SFA.DAS.GovUK.Auth.Services
{
    internal class OidcService : IOidcService
    {
        private readonly HttpClient _httpClient;
        private readonly IAzureIdentityService _azureIdentityService;
        private readonly IJwtSecurityTokenService _jwtSecurityTokenService;
        private readonly ICustomClaims _customClaims;
        private readonly GovUkOidcConfiguration _configuration;

        public OidcService(
            HttpClient httpClient,
            IAzureIdentityService azureIdentityService,
            IJwtSecurityTokenService jwtSecurityTokenService,
            GovUkOidcConfiguration configuration,
            ICustomClaims customClaims)
        {
            _httpClient = httpClient;
            _azureIdentityService = azureIdentityService;
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
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", openIdConnectMessage?.Code ?? ""),
                new KeyValuePair<string, string>("redirect_uri", openIdConnectMessage?.RedirectUri ?? ""),
                new KeyValuePair<string, string>("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
                new KeyValuePair<string, string>("client_assertion", CreateJwtAssertion()),
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

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "/userinfo")
            {

                Headers =
                {
                    UserAgent = {new ProductInfoHeaderValue("DfEApprenticeships", "1")},
                    Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
                }
            };
            var response = await _httpClient.SendAsync(httpRequestMessage);
            var valueString = response.Content.ReadAsStringAsync().Result;
            var content = JsonSerializer.Deserialize<GovUkUser>(valueString);
            if (content?.Email != null)
            {
                tokenValidatedContext.Principal.Identities.First().AddClaim(new Claim(ClaimTypes.Email, content.Email));
            }

            tokenValidatedContext.Principal.Identities.First()
                .AddClaims(await _customClaims.GetClaims(tokenValidatedContext));

        }

        private string CreateJwtAssertion()
        {
            var jti = Guid.NewGuid().ToString();
            var claimsIdentity = new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim("sub", _configuration.ClientId),
                    new Claim("jti", jti)

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
            var value = _jwtSecurityTokenService.CreateToken(_configuration.ClientId,
                $"{_configuration.BaseUrl}/token", claimsIdentity, signingCredentials);

            return value;
        }
    }
}