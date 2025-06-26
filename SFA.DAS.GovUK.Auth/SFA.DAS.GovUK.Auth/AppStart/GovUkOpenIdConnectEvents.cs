using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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
        private readonly string _redirectUrl;

        public GovUkOpenIdConnectEvents(
            IOptions<GovUkOidcConfiguration> config,
            IGovUkAuthenticationService govUkAuthenticationService,
            ICoreIdentityJwtValidator coreIdentityJwtValidator,
            string redirectUrl)
        {
            _config = config.Value;
            _govUkAuthenticationService = govUkAuthenticationService;
            _jwtValidator = coreIdentityJwtValidator;
            _redirectUrl = redirectUrl;
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

            if (context.ProtocolMessage.State == null)
            {
                context.ProtocolMessage.State = context.Options.StateDataFormat.Protect(context.Properties);
            }

            return Task.CompletedTask;
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
            context.Response.Redirect(_redirectUrl);
            context.HandleResponse();
            return Task.CompletedTask;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            await _govUkAuthenticationService.PopulateAccountClaims(context);
        }
    }
}