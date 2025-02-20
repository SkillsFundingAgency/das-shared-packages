using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Api.Models;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Constants;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Extensions;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("SFA.DAS.DfESignIn.Auth.UnitTests")]

namespace SFA.DAS.DfESignIn.Auth.Services
{
    internal class DfESignInService(
        IOptions<DfEOidcConfiguration> configuration,
        IApiHelper apiHelper,
        ICustomServiceRole customServiceRole)
        : IDfESignInService
    {
        private readonly DfEOidcConfiguration _configuration = configuration.Value;

        public async Task PopulateAccountClaims(TokenValidatedContext ctx)
        {
            var userOrganisation = JsonConvert.DeserializeObject<Organisation>
            (
                ctx.Principal.GetClaimValue(ClaimName.Organisation)
            );

            if (userOrganisation != null && ctx.Principal != null)
            {
                var userId = ctx.Principal.GetClaimValue(ClaimTypes.NameIdentifier);
                var ukPrn = userOrganisation.UkPrn?.ToString() ?? "0";

                if (userId != null)
                    await PopulateDfEClaims(ctx, userId, Convert.ToString(userOrganisation.Id));

                var displayName = $"{ctx.Principal.GetClaimValue(ClaimName.GivenName)} {ctx.Principal.GetClaimValue(ClaimName.FamilyName)}";

                ctx.HttpContext.Items.Add(ClaimsIdentity.DefaultNameClaimType, userId);
                ctx.HttpContext.Items.Add(CustomClaimsIdentity.DisplayName, displayName);

                ctx.Principal.Identities.First().AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, displayName));
                ctx.Principal.Identities.First().AddClaim(new Claim(CustomClaimsIdentity.DisplayName, displayName));
                ctx.Principal.Identities.First().AddClaim(new Claim(CustomClaimsIdentity.UkPrn, ukPrn));
            }
        }

        private async Task PopulateDfEClaims(TokenValidatedContext ctx, string userId, string userOrgId)
        {
            var response = await apiHelper.Get<ApiServiceResponse>($"{_configuration.APIServiceUrl}/services/{_configuration.APIServiceId}/organisations/{userOrgId}/users/{userId}");

            if (response != null)
            {
                var roleClaims = new List<Claim>();

                // Iterate the roles which are of only active status.
                foreach (var role in response.Roles.Where(role => role.Status.Id.Equals((int)RoleStatus.Active)))
                {
                    roleClaims.Add(new Claim(ClaimName.RoleCode, role.Code, ClaimTypes.Role, ctx.Options.ClientId));
                    roleClaims.Add(new Claim(ClaimName.RoleId, role.Id.ToString(), ClaimTypes.Role, ctx.Options.ClientId));
                    roleClaims.Add(new Claim(ClaimName.RoleName, role.Name, ClaimTypes.Role, ctx.Options.ClientId));
                    roleClaims.Add(new Claim(ClaimName.RoleNumericId, role.NumericId.ToString(), ClaimTypes.Role, ctx.Options.ClientId));

                    // Add to initial identity
                    // Check if the custom service role type is set in client side if not use the default CustomClaimsIdentity.Service
                    ctx.Principal?.Identities
                        .First()
                        .AddClaim(
                            new Claim(
                                type: customServiceRole.RoleClaimType ?? CustomClaimsIdentity.Service,
                                value: customServiceRole.RoleValueType.Equals(CustomServiceRoleValueType.Name)
                                    ? role.Name
                                    : role.Code));
                }
                ctx?.Principal?.Identities.First().AddClaims(roleClaims);
            }
        }
    }
}
