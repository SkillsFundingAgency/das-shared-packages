using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Extensions;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.GovUK.Auth.Services;

public class StubAuthenticationService : IStubAuthenticationService
{
    private readonly ICustomClaims _customClaims;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly GovUkOidcConfiguration _govUkOidcConfiguration;
    private readonly string _environment;

    public StubAuthenticationService(IConfiguration configuration, GovUkOidcConfiguration govUkOidcConfiguration, ICustomClaims customClaims, IHttpContextAccessor httpContextAccessor)
    {
        _customClaims = customClaims;
        _httpContextAccessor = httpContextAccessor;
        _govUkOidcConfiguration = govUkOidcConfiguration;
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

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(claimsIdentity);

        var userInfoClaimKeys = _govUkOidcConfiguration.RequestedUserInfoClaims
            .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var key in userInfoClaimKeys)
        {
            if (Enum.TryParse<UserInfoClaims>(key, out var enumValue))
            {
                if (enumValue == UserInfoClaims.CoreIdentityJWT)
                {
                    claimsIdentity
                        .AddClaim(
                            new Claim(enumValue.GetDescription(),
                            CoreIdentityJwtConverter.SerializeStubCoreIdentityJwt(model.GovUkUser.CoreIdentityJwt)));

                    claimsIdentity
                        .AddClaim(
                            new Claim("vot", "Cl.Cm.P2"));
                }

                if (enumValue == UserInfoClaims.Address)
                {
                    claimsIdentity
                        .AddClaim(
                            new Claim(enumValue.GetDescription(),
                            JsonSerializer.Serialize(model.GovUkUser.Addresses)));
                }

                if (enumValue == UserInfoClaims.Passport)
                {
                    claimsIdentity
                        .AddClaim(
                            new Claim(enumValue.GetDescription(),
                            JsonSerializer.Serialize(model.GovUkUser.Passports)));
                }

                if (enumValue == UserInfoClaims.DrivingPermit)
                {
                    claimsIdentity
                        .AddClaim(
                            new Claim(enumValue.GetDescription(),
                            JsonSerializer.Serialize(model.GovUkUser.DrivingPermits)));
                }

                if (enumValue == UserInfoClaims.ReturnCode)
                {
                    claimsIdentity
                        .AddClaim(
                            new Claim(enumValue.GetDescription(),
                            JsonSerializer.Serialize(model.GovUkUser.ReturnCodes)));
                }
            }
        }

        if (_customClaims != null)
        {
            claimsIdentity
                .AddClaims(await _customClaims.GetClaims(principal));
        }

        return principal;
    }

    public async Task<Token> GetToken(OpenIdConnectMessage openIdConnectMessage)
    {
        return await Task.FromResult(new Token { AccessToken = "stub-token" });
    }

    public Task PopulateAccountClaims(TokenValidatedContext tokenValidatedContext)
    {
        return Task.CompletedTask;
    }

    private bool TryGetUserInfoClaim(UserInfoClaims userInfoClaim, out string value) 
    {
        value = _httpContextAccessor.HttpContext.User.Claims
            .SingleOrDefault(p => p.Type == userInfoClaim.GetDescription())?.Value;

        return value != null;
    }
}