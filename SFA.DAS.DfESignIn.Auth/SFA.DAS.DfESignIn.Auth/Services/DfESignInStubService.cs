using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Constants;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Interfaces;

namespace SFA.DAS.DfESignIn.Auth.Services
{
    internal class DfESignInStubService : IDfESignInStubService
    {
        private readonly DfESignInStubConfiguration _stubConfiguration;
        private readonly DfEOidcConfiguration _configuration;
        private readonly ICustomServiceRole _customServiceRole;

        public DfESignInStubService(
            IOptions<DfESignInStubConfiguration> stubConfiguration,
            IOptions<DfEOidcConfiguration> configuration,
            ICustomServiceRole customServiceRole)
        {
            _stubConfiguration = stubConfiguration.Value;
            _configuration = configuration.Value;
            _customServiceRole = customServiceRole;
        }

        public ClaimsPrincipal GetStubSignInClaims(
            string userId,
            Guid organisationId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            var user = _stubConfiguration.Users.FirstOrDefault(
                configuredUser => configuredUser.Id == userId);

            var organisation = user?.Organisations.FirstOrDefault(
                configuredOrganisation =>
                    configuredOrganisation.Id == organisationId);

            if (organisation == null)
            {
                return null;
            }

            var displayName = $"{user.GivenName} {user.FamilyName}";

            var organisationClaim = JsonConvert.SerializeObject(new
            {
                id = organisation.Id,
                name = organisation.Name,
                ukprn = organisation.UkPrn?.ToString()
            });

            var claims = new List<Claim>
            {
                new Claim(ClaimName.Sub, user.Id),
                new Claim(ClaimName.Email, user.Email),
                new Claim(ClaimName.GivenName, user.GivenName),
                new Claim(ClaimName.FamilyName, user.FamilyName),
                new Claim(ClaimName.Organisation, organisationClaim),
                new Claim(ClaimsIdentity.DefaultNameClaimType, displayName),
                new Claim(CustomClaimsIdentity.DisplayName, displayName),
                new Claim(CustomClaimsIdentity.UkPrn, organisation.UkPrn?.ToString() ?? "0")
            };

            foreach (var role in organisation.Roles.Where(
                         role => role.Status.Id == (int)RoleStatus.Active))
            {
                claims.Add(new Claim(
                    ClaimName.RoleCode,
                    role.Code,
                    ClaimTypes.Role,
                    _configuration.ClientId));

                claims.Add(new Claim(
                    ClaimName.RoleId,
                    role.Id.ToString(),
                    ClaimTypes.Role,
                    _configuration.ClientId));

                claims.Add(new Claim(
                    ClaimName.RoleName,
                    role.Name,
                    ClaimTypes.Role,
                    _configuration.ClientId));

                claims.Add(new Claim(
                    ClaimName.RoleNumericId,
                    role.NumericId.ToString(),
                    ClaimTypes.Role,
                    _configuration.ClientId));

                claims.Add(new Claim(
                    _customServiceRole.RoleClaimType
                        ?? CustomClaimsIdentity.Service,
                    _customServiceRole.RoleValueType
                        == CustomServiceRoleValueType.Name
                            ? role.Name
                            : role.Code));
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            return new ClaimsPrincipal(identity);
        }
    }
}