using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using SFA.DAS.OidcMiddleware.Builders;
using SFA.DAS.OidcMiddleware.Validators;

namespace SFA.DAS.OidcMiddleware.Strategies
{
    public class AuthenticateCoreStrategy : IAuthenticateCoreStrategy
    {
        private readonly OidcMiddlewareOptions _options;
        private readonly IBuildRequestAuthorisationCode _buildRequestAuthorisationCode;
        private readonly IBuildUserInfoClientUrl _buildUserInfoClient;
        private readonly ISecurityTokenValidation _securityTokenValidation;

        public AuthenticateCoreStrategy(OidcMiddlewareOptions options, IBuildRequestAuthorisationCode buildRequestAuthorisationCode, IBuildUserInfoClientUrl buildUserInfoClient, ISecurityTokenValidation securityTokenValidation)
        {
            _options = options;
            _buildRequestAuthorisationCode = buildRequestAuthorisationCode;
            _buildUserInfoClient = buildUserInfoClient;
            _securityTokenValidation = securityTokenValidation;
        }

        public async Task<AuthenticationTicket> Authenticate(IOwinContext context)
        {

            var query = context.Request.Query;

            if (query?.GetValues("code") == null || query?.GetValues("state") == null)
            {
                return new AuthenticationTicket(null, null);
            }

            var code = query.GetValues("code")[0];
            var tokenResponse = await _buildRequestAuthorisationCode.GetTokenResponse(_options.TokenEndpoint, _options.ClientId, _options.ClientSecret, code, context.Request.Uri);

            var tempState = await GetTempStateAsync(context);
            if (tempState == null)
            {
                return new AuthenticationTicket(null, null);
            }
            context.Authentication.SignOut("TempState");

            var identity = await ValidateResponseAndSignInAsync(tokenResponse, tempState.Item2, context);

            var state = query.GetValues("state")[0];
            var properties = _options.StateDataFormat.Unprotect(state);
            _options.AuthenticatedCallback?.Invoke(identity);


            return new AuthenticationTicket(identity, properties);
        }

        private async Task<Tuple<string, string>> GetTempStateAsync(IOwinContext context)
        {
            var data = await context.Authentication.AuthenticateAsync("TempState");
            if (data == null) return null;
            var state = data.Identity.FindFirst("state").Value;
            var nonce = data.Identity.FindFirst("nonce").Value;

            return Tuple.Create(state, nonce);
        }

        private async Task<ClaimsIdentity> ValidateResponseAndSignInAsync(TokenResponse response, string nonce, IOwinContext context)
        {
            if (!string.IsNullOrWhiteSpace(response.IdentityToken))
            {
                var claims = _securityTokenValidation.ValidateToken(response.IdentityToken, nonce);

                if (!string.IsNullOrWhiteSpace(response.AccessToken))
                {
                    claims.AddRange(await _buildUserInfoClient.GetUserClaims(_options, response.AccessToken));
                    claims.Add(new Claim("access_token", response.AccessToken));
                    claims.Add(new Claim("expires_at", (DateTime.UtcNow.ToEpochTime() + response.ExpiresIn).ToDateTimeFromEpoch().ToString()));
                }

                if (!string.IsNullOrWhiteSpace(response.RefreshToken))
                {
                    claims.Add(new Claim("refresh_token", response.RefreshToken));
                }

                var id = new ClaimsIdentity(claims, "Cookies");

                context.Authentication.SignIn(id);
          
                return id;
            }

            return null;
        }
    }
}
