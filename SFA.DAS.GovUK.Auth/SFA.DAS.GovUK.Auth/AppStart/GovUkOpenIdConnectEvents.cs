using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Linq;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Extensions;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.GovUK.Auth.Validation;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    public class GovUkOpenIdConnectEvents : OpenIdConnectEvents
    {
        private readonly GovUkOidcConfiguration _config;
        private readonly IGovUkAuthenticationService _govUkAuthenticationService;
        private readonly ICoreIdentityJwtValidator _jwtValidator;
        private readonly ISigningCredentialsProvider _signingCredentialsProvider;
        private readonly string _signedOutRedirectUrl;
        private readonly string _suspendedRedirectUrl;

        public GovUkOpenIdConnectEvents(
            IOptions<GovUkOidcConfiguration> config,
            IGovUkAuthenticationService govUkAuthenticationService,
            ICoreIdentityJwtValidator coreIdentityJwtValidator,
            ISigningCredentialsProvider signingCredentialsProvider,
            string signedOutRedirectUrl,
            string suspendedRedirectUrl)
        {
            _config = config.Value;
            _govUkAuthenticationService = govUkAuthenticationService;
            _jwtValidator = coreIdentityJwtValidator;
            _signingCredentialsProvider = signingCredentialsProvider;
            _signedOutRedirectUrl = signedOutRedirectUrl;
            _suspendedRedirectUrl = suspendedRedirectUrl;
        }

        public override Task RemoteFailure(RemoteFailureContext context)
        {
            if (context.Failure != null && context.Failure.Message.Contains("Correlation failed"))
            {
                context.Response.Redirect("/");
                context.HandleResponse();
            }

            return Task.CompletedTask;
        }

        public override Task RedirectToIdentityProvider(RedirectContext context)
        {
            var disable2Fa = AuthenticationExtension.Disable2Fa(_config);
            var enableVerify = AuthenticationExtension.EnableVerify(_config, context.Properties);

            var vtr = disable2Fa ? "Cl" : "Cl.Cm";
            var stringVtr = JsonSerializer.Serialize(new List<string>
                            {
                                enableVerify ? vtr + ".P2" : vtr
                            });

            if (context.ProtocolMessage.Parameters.ContainsKey("vtr"))
            {
                context.ProtocolMessage.Parameters.Remove("vtr");
            }

            context.ProtocolMessage.Parameters.Add("vtr", stringVtr);

            if (enableVerify)
            {
                var userInfoClaimKeys = _config.RequestedUserInfoClaims
                    .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                var claims = new Dictionary<string, Dictionary<string, object>>
                {
                    ["userinfo"] = new Dictionary<string, object>()
                };

                foreach (var key in userInfoClaimKeys)
                {
                    if (Enum.TryParse<UserInfoClaims>(key, out var enumValue))
                    {
                        var description = enumValue.GetDescription();
                        if (!string.IsNullOrEmpty(description))
                        {
                            claims["userinfo"][description] = null;
                        }
                    }
                }

                context.ProtocolMessage.Parameters.Add("claims", JsonSerializer.Serialize(claims));
            }

            var jarState = context.Options.StateDataFormat.Protect(
                new AuthenticationProperties(new Dictionary<string, string>(context.Properties.Items))
            {
                Items =
                {
                    [OpenIdConnectDefaults.RedirectUriForCodePropertiesKey] = context.ProtocolMessage.RedirectUri
                }
            });

            context.ProtocolMessage.Parameters["request"] = CreateJarRequestObject(context, jarState);
            return Task.CompletedTask;
        }

        private string CreateJarRequestObject(RedirectContext context, string state)
        {
            var msg = context.ProtocolMessage;
            var now = DateTime.UtcNow;

            var payload = new JwtPayload(
                issuer: _config.ClientId,
                audience: OneLoginUrlHelper.GetJarAudience(_config.BaseUrl),
                claims: null,
                notBefore: now,
                expires: now.AddMinutes(5),
                issuedAt: now)
            {
                ["response_type"] = msg.ResponseType,
                ["client_id"] = msg.ClientId,
                ["redirect_uri"] = msg.RedirectUri,
                ["scope"] = msg.Scope,
                ["state"] = state,
                ["nonce"] = msg.Nonce,
                ["ui_locales"] = "en"
            };

            if (msg.Parameters.TryGetValue("vtr", out var vtrJson))
            {
                payload["vtr"] = JArray.Parse(vtrJson);
            }
            if (msg.Parameters.TryGetValue("claims", out var claimsJson))
            {
                payload["claims"] = JObject.Parse(claimsJson);
            }
            if (msg.Parameters.TryGetValue("code_challenge", out var codeChallenge))
            {
                payload["code_challenge"] = codeChallenge;
            }
            if (msg.Parameters.TryGetValue("code_challenge_method", out var codeChallengeMethod))
            {
                payload["code_challenge_method"] = codeChallengeMethod;
            }

            var header = new JwtHeader(_signingCredentialsProvider.GetSigningCredentials());
            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(header, payload));
        }

        public override async Task TokenResponseReceived(TokenResponseReceivedContext context)
        {
            if (context.TokenEndpointResponse.IdToken is string idToken)
            {
                context.Properties?.StoreTokens(
                [
                    new AuthenticationToken
                                    {
                                        Name = OpenIdConnectParameterNames.IdToken,
                                        Value = idToken
                                    }
                ]);
            }

            if (AuthenticationExtension.EnableVerify(_config, context.Properties))
            {
                await _jwtValidator.LoadDidDocument();
            }
        }

        public override async Task AuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            var token = await _govUkAuthenticationService.GetToken(context.TokenEndpointRequest);
            if (token?.AccessToken != null && token.IdToken != null)
            {
                context.Properties.StoreTokens(new[]
                {
                                    new AuthenticationToken { Name = "access_token", Value = token.AccessToken },
                                    new AuthenticationToken { Name = "id_token", Value = token.IdToken },
                                });

                context.HandleCodeRedemption(token.AccessToken, token.IdToken);
            }
        }

        public override Task SignedOutCallbackRedirect(RemoteSignOutContext context)
        {
            context.Response.Cookies.Delete(GovUkConstants.AuthCookieName);
            context.Response.Redirect(_signedOutRedirectUrl);
            context.HandleResponse();
            return Task.CompletedTask;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            await _govUkAuthenticationService.PopulateAccountClaims(context);

            context.Properties ??= new AuthenticationProperties();
            context.Properties.Items["suspended_redirect"] = _suspendedRedirectUrl;
        }
    }
}