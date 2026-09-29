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
using Microsoft.Extensions.Configuration;
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
    private readonly string _environment;

    public StubAuthenticationService(IConfiguration configuration, IOptions<GovUkOidcConfiguration> config, ICustomClaims customClaims, IHttpContextAccessor httpContextAccessor)
    {
        _config = config.Value;
        _customClaims = customClaims;
        _httpContextAccessor = httpContextAccessor;
        _environment = configuration["ResourceEnvironmentName"]?.ToUpper();
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

    public async Task<StubSignInResult> GetStubSignInClaims(StubAuthUserDetails model)
    {
        if (_environment.Equals("PRD", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Stub sign-in is disabled in production.");
        }

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

        var govUkUser = new GovUkUser
        {
            Sub = model.Id,
            Email = model.Email,
            EmailVerified = model.GovUkUser?.EmailVerified ?? true,
            PhoneNumber = model.Mobile,
            PhoneNumberVerified = model.GovUkUser?.PhoneNumberVerified ?? !string.IsNullOrWhiteSpace(model.Mobile),
            CoreIdentityJwt = model.GovUkUser?.CoreIdentityJwt,
            Addresses = model.GovUkUser?.Addresses,
            DrivingPermits = model.GovUkUser?.DrivingPermits,
            Passports = model.GovUkUser?.Passports,
            ReturnCodes = model.GovUkUser?.ReturnCodes
        };

        claims.Add(new Claim(GovUkUserClaimTypes.UserInfo, JsonSerializer.Serialize(govUkUser)));

        if (govUkUser.CoreIdentityJwt?.Vc?.CredentialSubject != null)
        {
            claims.Add(new Claim(GovUkUserClaimTypes.VerifiedIdentity, "true"));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(claimsIdentity);

        if (_customClaims != null)
        {
            var context = new TokenValidatedContext(
                _httpContextAccessor.HttpContext!,
                new AuthenticationScheme(
                    OpenIdConnectDefaults.AuthenticationScheme,
                    "Stub",
                    typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions(),
                principal,
                new AuthenticationProperties());

            var additionalClaims = await _customClaims.GetClaims(context);

            if (context.Result?.Handled == true)
            {
                // the custom handler has already written the response, e.g. Response.Redirect("/Home/AccessDenied")
                return new StubSignInResult
                {
                    Principal = null,
                    ResponseHandled = true
                };
            }

            if (context.Result != null)
            {
                throw new InvalidOperationException("Custom claims handler stopped stub sign-in.");
            }

            principal = context.Principal
                ?? throw new InvalidOperationException("Custom claims handler removed the principal.");

            claimsIdentity = principal.Identity as ClaimsIdentity
                ?? throw new InvalidOperationException("Custom claims handler returned a principal without a claims identity.");

            claimsIdentity.AddClaims(additionalClaims);
        }

        return new StubSignInResult
        {
            Principal = principal,
            ResponseHandled = false
        };
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