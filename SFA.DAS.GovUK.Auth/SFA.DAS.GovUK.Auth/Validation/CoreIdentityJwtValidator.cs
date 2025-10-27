using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Exceptions;
using SFA.DAS.GovUK.Auth.Helper;

namespace SFA.DAS.GovUK.Auth.Validation
{
    public sealed class CoreIdentityJwtValidator : ICoreIdentityJwtValidator
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILogger<CoreIdentityJwtValidator> _logger;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1); 
        private Did _did;
        private DateTimeOffset? _didExpires;

        public CoreIdentityJwtValidator(IHttpClientFactory httpClientFactory, GovUkOidcConfiguration config, IDateTimeHelper dateTimeHelper, ILogger<CoreIdentityJwtValidator> logger)
        {
            ArgumentNullException.ThrowIfNull(httpClientFactory);
            ArgumentNullException.ThrowIfNull(config);
            ArgumentNullException.ThrowIfNull(logger);

            _httpClientFactory = httpClientFactory;
            _baseUrl = config.BaseUrl;
            _dateTimeHelper = dateTimeHelper;
            _logger = logger;

            _tokenHandler = new JwtSecurityTokenHandler
            {
                MapInboundClaims = false
            };
        }

        public ClaimsPrincipal ValidateCoreIdentity(string coreIdentityJwt)
        {
            if (_did is null)
            {
                throw new InvalidOperationException("The DID has not been fetched.");
            }

            var coreIdentityClaimIssuer = OneLoginUrlHelper.GetCoreIdentityClaimIssuer(_baseUrl);

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = coreIdentityClaimIssuer,
                ValidateAudience = false,
                NameClaimType = "sub",
                IssuerSigningKeyResolver = (string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters) =>
                {
                    var controllerId = kid.Split('#')[0];
                    if (controllerId != _did.Id)
                    {
                        return Enumerable.Empty<SecurityKey>();
                    }

                    var assertionMethods = _did.JwkAssertionMethods;
                    return assertionMethods.Where(am => am.Id == kid).Select(am => am.Key);
                }
            };

            var coreIdentityPrincipal = _tokenHandler.ValidateToken(coreIdentityJwt, tokenValidationParameters, out var securityKey);

            var idToken = securityKey as JwtSecurityToken;
            var vot = idToken?.Claims.FirstOrDefault(c => c.Type == "vot")?.Value;

            if (vot == null || !vot.Contains("P2"))
            {
                throw new SecurityTokenException("The vot claim does not exist or does not contain the required level of confidence");
            }

            return coreIdentityPrincipal;
        }

        public async Task LoadDidDocument()
        {
            if (_did is null)
            {
                await RefreshDid();
            }
            else if (_didExpires is DateTimeOffset expires && expires < _dateTimeHelper.UtcNowOffset)
            {
                try
                {
                    await RefreshDid();
                }
                catch (Exception ex)
                {
                    // a cached document should be used if refreshing the document fails
                    // so do not bubble up any exceptions
                    _logger.LogWarning(ex, "Failed to refresh DID document.");
                }
            }
        }

        private async Task RefreshDid()
        {
            var endpoint = OneLoginUrlHelper.GetDidEndpoint(_baseUrl);

            try
            {
                await _lock.WaitAsync();

                // creating a client with a specific user agent as unknown agents are forbidden (403)
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("SFA.Apprenticeships/1.0 (+https://www.gov.uk)");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                DateTimeOffset? didDocumentExpires = null;
                if (response.Headers.CacheControl?.MaxAge is TimeSpan maxAge)
                {
                    didDocumentExpires = _dateTimeHelper.UtcNowOffset.Add(maxAge);
                }

                using var document = JsonSerializer.Deserialize<JsonDocument>(await response.Content.ReadAsStringAsync())!;

                if (!document.RootElement.TryGetProperty("id", out var docIdElement) || docIdElement.ValueKind != JsonValueKind.String)
                {
                    throw new DidLoadException("DID does not contain an 'id' string property.");
                }
                if (!document.RootElement.TryGetProperty("assertionMethod", out var assertionMethodsElement) || assertionMethodsElement.ValueKind != JsonValueKind.Array)
                {
                    throw new DidLoadException("DID does not contain an 'assertionMethod' array property.");
                }

                var docId = docIdElement.GetString()!;

                var assertionMethods = new List<JwkAssertionMethod>();

                foreach (var assertionMethod in assertionMethodsElement.EnumerateArray())
                {
                    var type = assertionMethod.GetProperty("type").GetString();
                    if (type != "JsonWebKey")
                    {
                        continue;
                    }

                    if (assertionMethod.TryGetProperty("id", out var idElement) && idElement.ValueKind == JsonValueKind.String &&
                        assertionMethod.TryGetProperty("controller", out var controllerElement) && controllerElement.ValueKind == JsonValueKind.String &&
                        assertionMethod.TryGetProperty("publicKeyJwk", out var publicKeyJwkElement) && publicKeyJwkElement.ValueKind == JsonValueKind.Object)
                    {
                        var key = JsonWebKey.Create(publicKeyJwkElement.ToString());

                        assertionMethods.Add(new JwkAssertionMethod(idElement.GetString()!, controllerElement.GetString()!, key));
                    }
                }

                _did = new Did(docId, assertionMethods.ToArray());
                _didExpires = didDocumentExpires;
            }
            finally
            {
                _lock.Release();
            }
        }

        private sealed class Did
        {
            public Did(string id, JwkAssertionMethod[] jwkAssertionMethods)
            {
                Id = id;
                JwkAssertionMethods = jwkAssertionMethods;
            }

            public string Id { get; }
            public JwkAssertionMethod[] JwkAssertionMethods { get; }
        }

        private sealed class JwkAssertionMethod
        {
            public JwkAssertionMethod(string id, string controller, JsonWebKey key)
            {
                Id = id;
                Controller = controller;
                Key = key;
            }

            public string Id { get; }
            public string Controller { get; }
            public JsonWebKey Key { get; }
        }
    }
}
