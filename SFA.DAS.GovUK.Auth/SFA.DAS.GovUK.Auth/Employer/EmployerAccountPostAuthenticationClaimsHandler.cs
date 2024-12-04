using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Newtonsoft.Json;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.Auth.Employer;

public class EmployerAccountPostAuthenticationClaimsHandler(IGovAuthEmployerAccountService accountsService) : ICustomClaims
{
    // To allow unit testing
    public int MaxPermittedNumberOfAccountsOnClaim { get; set; } = Constants.MaxNumberOfAssociatedAccountsAllowedOnClaim;

    public async Task<IEnumerable<Claim>> GetClaims(TokenValidatedContext tokenValidatedContext)
    {
        var claims = new List<Claim>();
        var userId = tokenValidatedContext!.Principal!.Claims
            .First(c => c.Type.Equals(ClaimTypes.NameIdentifier))
            .Value;
        var email = tokenValidatedContext.Principal.Claims
            .First(c => c.Type.Equals(ClaimTypes.Email))
            .Value;

        var result = await accountsService.GetUserAccounts(userId, email);

        if (result.IsSuspended)
        {
            claims.Add(new Claim(ClaimTypes.AuthorizationDecision, "Suspended"));
        }

        if (!string.IsNullOrEmpty(result.FirstName) && !string.IsNullOrEmpty(result.LastName))
        {
            claims.Add(new Claim(EmployerClaims.GivenName, result.FirstName));
            claims.Add(new Claim(EmployerClaims.FamilyName, result.LastName));
            claims.Add(new Claim(EmployerClaims.IdamsUserDisplayNameClaimTypeIdentifier, result.FirstName + " " + result.LastName));
        }

        claims.Add(new Claim(EmployerClaims.IdamsUserIdClaimTypeIdentifier, result.EmployerUserId));
        claims.Add(new Claim(EmployerClaims.IdamsUserEmailClaimTypeIdentifier, email));

        result.EmployerAccounts
            .Where(c => c.Role.Equals("owner", StringComparison.CurrentCultureIgnoreCase) || c.Role.Equals("transactor", StringComparison.CurrentCultureIgnoreCase))
            .ToList().ForEach(u => claims.Add(new Claim(EmployerClaims.Account, u.AccountId)));

        // Some users have 100's of employer accounts. The claims cannot handle that volume of data, it will cause exceptions.
        // If that is the case, we will still add the claim for authorization purposes but leave it empty.
        var accountsAsJson = JsonConvert.SerializeObject(result.EmployerAccounts.Count() <= MaxPermittedNumberOfAccountsOnClaim
            ? result.EmployerAccounts.ToDictionary(k => k.AccountId)
            : new List<EmployerUserAccountItem>().ToDictionary(k => k.AccountId));

        var associatedAccountsClaim = new Claim(EmployerClaims.AccountsClaimsTypeIdentifier, accountsAsJson, JsonClaimValueTypes.Json);

        claims.Add(associatedAccountsClaim);

        return claims;
    }
}