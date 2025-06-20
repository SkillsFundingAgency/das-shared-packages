using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Exceptions;
using SFA.DAS.GovUK.Auth.Extensions;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.GovUK.Auth.Services;

public class StubAuthenticationService : IStubAuthenticationService
{
    public const string StubGovUkUserClaimType = nameof(StubGovUkUserClaimType);

    private readonly ICustomClaims _customClaims;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _environment;

    public StubAuthenticationService(IConfiguration configuration, ICustomClaims customClaims, IHttpContextAccessor httpContextAccessor)
    {
        _customClaims = customClaims;
        _httpContextAccessor = httpContextAccessor;
        _environment = configuration["ResourceEnvironmentName"]?.ToUpper();
    }

    public async Task<GovUkUser> GetAccountDetails(string accessToken)
    {
        var govUkUser = new GovUkUser
        {
            Sub = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(p => p.Type == "sub")?.Value,
            Email = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(p => p.Type == ClaimTypes.Email)?.Value,
            EmailVerified = true,
            PhoneNumber = _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(p => p.Type == ClaimTypes.MobilePhone)?.Value,
            PhoneNumberVerified = true,
            CoreIdentityJwt = TryGetUserInfoClaim(UserInfoClaims.CoreIdentityJWT, out string coreIdentityJwt) ? JsonSerializer.Deserialize<GovUkCoreIdentityJwt>(JsonSerializer.Serialize(coreIdentityJwt)) : null,
            Addresses = TryGetUserInfoClaim(UserInfoClaims.Address, out string addresses) ? JsonSerializer.Deserialize<List<GovUkAddress>>(addresses) : null,
            DrivingPermits = TryGetUserInfoClaim(UserInfoClaims.DrivingPermit, out string drivingPermits) ? JsonSerializer.Deserialize<List<GovUkDrivingPermit>>(drivingPermits) : null,
            Passports = TryGetUserInfoClaim(UserInfoClaims.Passport, out string passports) ? JsonSerializer.Deserialize<List<GovUkPassport>>(passports) : null,
            ReturnCodes = TryGetUserInfoClaim(UserInfoClaims.ReturnCode, out string returnCodes) ? JsonSerializer.Deserialize<List<GovUkReturnCode>>(returnCodes) : null
        };

        return await Task.FromResult(govUkUser);
    }

    public async Task<ClaimsPrincipal> GetStubSignInClaims(StubAuthUserDetails model)
    {
        if (_environment == "PRD")
        {
            return null;
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

        if(model.GovUkUser != null)
        {
            claims.Add(new Claim(StubGovUkUserClaimType, JsonSerializer.Serialize(model.GovUkUser)));
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

                if (rootNode.TryGetPropertyValue("https://vocab.account.gov.uk/v1/coreIdentityJWT", out var unwrappedNode))
                {
                    var coreJwt = unwrappedNode.Deserialize<GovUkCoreIdentityJwt>();
                    var jwtString = CoreIdentityJwtConverter.SerializeStubCoreIdentityJwt(coreJwt);

                    rootNode.Remove("https://vocab.account.gov.uk/v1/coreIdentityJWT");
                    rootNode["https://vocab.account.gov.uk/v1/coreIdentityJWT"] = jwtString;
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

    private bool TryGetUserInfoClaim(UserInfoClaims userInfoClaim, out string value) 
    {
        value = _httpContextAccessor.HttpContext.User.Claims
            .SingleOrDefault(p => p.Type == userInfoClaim.GetDescription())?.Value;

        return value != null;
    }
}