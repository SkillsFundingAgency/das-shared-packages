using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SFA.DAS.ApiTokens.Client
{
    public class ApiKeyHandler : DelegatingHandler
    {
        private readonly string _apiKeyHeaderName;
        private readonly string _apiKeySecret;
        private readonly string _apiIssuer;
        private readonly IEnumerable<string> _apiAudiences;

        public ApiKeyHandler(string apiKeyHeaderName, string apiKeySecret, string apiIssuer, IEnumerable<string> apiAudiences)
        {
            _apiKeyHeaderName = apiKeyHeaderName;
            _apiKeySecret = apiKeySecret;
            _apiIssuer = apiIssuer;
            _apiAudiences = apiAudiences;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                if (!request.Headers.Contains(_apiKeyHeaderName)) return base.SendAsync(request, cancellationToken);

                var headerValue = request.Headers.GetValues(_apiKeyHeaderName).First();

                ValidateKeyAndSetPrincipal(headerValue);

                return base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                var invalidResponse = new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = ex.GetType().Name };
                var invalidTsc = new TaskCompletionSource<HttpResponseMessage>();
                invalidTsc.SetResult(invalidResponse);
                return invalidTsc.Task;
            }
        }

        private void ValidateKeyAndSetPrincipal(string headerValue)
        {
            var tokenService = new JwtTokenService(_apiKeySecret);

            var tokenValue = headerValue.Split(' ').ElementAt(1);
            var tokenAudience = tokenService.Decode(tokenValue, "aud", _apiAudiences, _apiIssuer);
            var tokenRoles = tokenService.Decode(tokenValue, "data", _apiAudiences, _apiIssuer);

            var roles = tokenRoles.Split(' ');

            var principal = new GenericPrincipal(new GenericIdentity(tokenAudience), roles);

            Thread.CurrentPrincipal = principal;
            HttpContext.Current.User = principal;
        }

        private class JwtTokenService
        {
            private readonly double _allowedClockSkewInMinutes;
            private readonly InMemorySymmetricSecurityKey _signingKey;

            public JwtTokenService(string secret, double allowedClockSkewInMinutes = 5)
            {
                _allowedClockSkewInMinutes = allowedClockSkewInMinutes;
                _signingKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            }

            public string Decode(string token, string claimName, IEnumerable<string> validAudiences, string validIssuer)
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudiences = validAudiences,
                    ValidIssuers = new[] { validIssuer },
                    IssuerSigningKey = _signingKey,
                    ClockSkew = TimeSpan.FromMinutes(_allowedClockSkewInMinutes)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;

                var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
                var claim = claimsPrincipal.FindFirst(x => x.Type == claimName);

                return claim.Value;
            }
        }
    }
}
