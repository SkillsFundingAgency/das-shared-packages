using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Exceptions;
using SFA.DAS.GovUK.Auth.Extensions;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.GovUK.Auth.Services;

public class StubAuthenticationService : IStubAuthenticationService
{
    private readonly GovUkOidcConfiguration _config;
    private readonly ICustomClaims _customClaims;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public StubAuthenticationService(IOptions<GovUkOidcConfiguration> config, ICustomClaims customClaims, IHttpContextAccessor httpContextAccessor)
    {
        _config = config.Value;
        _customClaims = customClaims;
        _httpContextAccessor = httpContextAccessor;
    }

    public GovUkUser GetAccountDetails()
    {
        var principal = _httpContextAccessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var json = principal.FindFirstValue(GovUkUserClaimTypes.UserInfo);

        var govUkUser = string.IsNullOrWhiteSpace(json)
            ? null
            : JsonSerializer.Deserialize<GovUkUser>(json);

        return govUkUser;
    }

    public async Task<ClaimsPrincipal> GetStubSignInClaims(StubAuthUserDetails model)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, model.Email),
            new(ClaimTypes.NameIdentifier, model.Id),
            new("sub", model.Id)
        };

        if (model.Mobile != null)
        {
            claims.Add(new Claim(ClaimTypes.MobilePhone, model.Mobile));
        }

        if(model.GovUkUser != null)
        {
            claims.Add(new Claim(GovUkUserClaimTypes.UserInfo, JsonSerializer.Serialize(model.GovUkUser)));

            var coreIdentity = model.GovUkUser.CoreIdentityJwt;

            if (coreIdentity?.Vc?.CredentialSubject != null)
            {
                claims.Add(new Claim(
                    GovUkUserClaimTypes.VerifiedIdentity,
                    "true"));
            }
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(claimsIdentity);

        if (_customClaims != null)
        {
            claimsIdentity
                .AddClaims(await _customClaims.GetClaims(principal));
        }

        return principal;
    }

    public async Task<GovUkUser> GetStubVerifyGovUkUser(IFormFile formFile)
    {
        if (formFile != null && formFile.Length > 0)
        {
            try
            {
                using var reader = new StreamReader(formFile.OpenReadStream());
                var json = await reader.ReadToEndAsync();

                var rootNode = JsonNode.Parse(json)?.AsObject();
                if (rootNode == null) throw new StubVerifyException("Invalid JSON structure.");

                if (rootNode.TryGetPropertyValue(UserInfoClaims.CoreIdentityJWT.GetDescription(), out var unwrappedNode))
                {
                    var coreJwt = unwrappedNode.Deserialize<GovUkCoreIdentityJwt>();
                    var jwtString = CoreIdentityJwtConverter.SerializeStubCoreIdentityJwt(coreJwt);

                    rootNode.Remove(UserInfoClaims.CoreIdentityJWT.GetDescription());
                    rootNode[UserInfoClaims.CoreIdentityJWT.GetDescription()] = jwtString;
                }

                // remove all the GovUkUser properties which are not configured to be returned from the /userInfo endpoint
                var keys = _config.RequestedUserInfoClaims.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                
                var configuredClaims = new HashSet<UserInfoClaims>();
                foreach (var key in keys)
                {
                    if (Enum.TryParse<UserInfoClaims>(key, true, out var claim))
                    {
                        configuredClaims.Add(claim);
                    }
                }

                foreach (var claim in Enum.GetValues<UserInfoClaims>())
                {
                    if (!configuredClaims.Contains(claim))
                    {
                        rootNode.Remove(claim.GetDescription());
                    }
                }

                var encryptedJson = rootNode.ToJsonString();
                return JsonSerializer.Deserialize<GovUkUser>(encryptedJson);
            }
            catch
            {
                throw new StubVerifyException("Invalid JSON file.");
            }
        }

        return null;
    }

    public async Task<Token> GetToken(OpenIdConnectMessage openIdConnectMessage)
    {
        return await Task.FromResult(new Token { AccessToken = "stub-token" });
    }

    public Task PopulateAccountClaims(TokenValidatedContext tokenValidatedContext)
    {
        return Task.CompletedTask;
    }

    public async Task<IActionResult> ChallengeWithVerifyAsync(string returnUrl, Controller controller)
    {
        var props = new AuthenticationProperties
        {
            RedirectUri = returnUrl,
            AllowRefresh = true
        };
        props.Items["enableVerify"] = true.ToString();

        var currentIdentity = (ClaimsIdentity)controller.User.Identity;
        var newIdentity = new ClaimsIdentity(currentIdentity.Claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await controller.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(newIdentity),
            props);

        return controller.LocalRedirect(returnUrl);
    }
}