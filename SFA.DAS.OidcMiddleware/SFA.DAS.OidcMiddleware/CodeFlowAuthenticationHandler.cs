using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using SFA.DAS.OidcMiddleware.Builders;
using SFA.DAS.OidcMiddleware.Caches;
using SFA.DAS.OidcMiddleware.Clients;
using SFA.DAS.OidcMiddleware.Validators;

namespace SFA.DAS.OidcMiddleware
{
    public class CodeFlowAuthenticationHandler : AuthenticationHandler<OidcMiddlewareOptions>
    {
        private readonly IAuthorizeUrlBuilder _authorizeUrlBuilder;
        private readonly INonceCache _nonceCache;
        private readonly ITokenClient _tokenClient;
        private readonly ITokenValidator _tokenValidator;
        private readonly IUserInfoClient _userInfoClient;

        public CodeFlowAuthenticationHandler(
            IAuthorizeUrlBuilder authorizeUrlBuilder, 
            INonceCache nonceCache, 
            ITokenClient tokenClient, 
            ITokenValidator tokenValidator, 
            IUserInfoClient userInfoClient)
        {
            _authorizeUrlBuilder = authorizeUrlBuilder;
            _nonceCache = nonceCache;
            _tokenClient = tokenClient;
            _tokenValidator = tokenValidator;
            _userInfoClient = userInfoClient;
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            var code = Request.Query.Get("code");
            var state = Request.Query.Get("state");

            if (code == null || state == null)
            {
                return null;
            }

            var properties = Options.StateDataFormat.Unprotect(state);

            if (properties == null)
            {
                return null;
            }

            var nonce = await _nonceCache.GetNonceAsync(Context.Authentication);

            if (nonce == null)
            {
                return null;
            }
            
            var httpClient = new HttpClient();
            var tokenResponse = await _tokenClient.RequestAuthorizationCodeAsync(httpClient, Options.TokenEndpoint, Options.ClientId, Options.ClientSecret, code, Request.Uri);

            if (tokenResponse.IsError)
            {
                throw new OidcAuthenticationException(tokenResponse.Error);
            }

            _tokenValidator.ValidateToken(Options, tokenResponse.IdentityToken, nonce);

            var identity = new ClaimsIdentity("Cookies");

            identity.AddClaim(new Claim("id_token", tokenResponse.IdentityToken));

            if (!string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                identity.AddClaims(await _userInfoClient.GetUserClaims(httpClient, Options, tokenResponse.AccessToken));
                identity.AddClaim(new Claim("access_token", tokenResponse.AccessToken));
                identity.AddClaim(new Claim("expires_at", (DateTime.UtcNow.ToEpochTime() + tokenResponse.ExpiresIn).ToDateTimeFromEpoch().ToString()));
            }

            if (!string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
            {
                identity.AddClaim(new Claim("refresh_token", tokenResponse.RefreshToken));
            }

            Options.AuthenticatedCallback?.Invoke(identity);

            return new AuthenticationTicket(identity, properties);
        }

        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode != 401)
            {
                return Task.FromResult<object>(null);
            }
            
            var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

            if (challenge == null)
            {
                return Task.FromResult<object>(null);
            }

            if (string.IsNullOrWhiteSpace(challenge.Properties.RedirectUri))
            {
                challenge.Properties.RedirectUri = Request.Uri.ToString();
            }

            var state = Options.StateDataFormat.Protect(challenge.Properties);
            var nonce = Guid.NewGuid().ToString("N");

            _nonceCache.SetNonceAsync(Context.Authentication, nonce);

            var url = _authorizeUrlBuilder.BuildAuthorizeUrl(Options.AuthorizeEndpoint, Options.ClientId, Options.Scopes, Request.Uri, state, nonce);

            Response.Redirect(url);

            return Task.FromResult<object>(null);
        }

        public override async Task<bool> InvokeAsync()
        {
            var ticket = await AuthenticateAsync();

            if (ticket == null)
            {
                return false;
            }

            Context.Authentication.SignIn(ticket.Properties, ticket.Identity);
            Response.Redirect(ticket.Properties.RedirectUri);

            return true;
        }
    }
}
