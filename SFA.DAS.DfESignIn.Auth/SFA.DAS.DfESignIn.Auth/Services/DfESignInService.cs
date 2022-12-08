using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.Api.Models;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Constants;
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
    internal class DfESignInService : IDfESignInService
    {
        private readonly DfEOidcConfiguration _configuration;
        private readonly ITokenDataSerializer _tokenDataSerializer;
        private readonly ITokenEncoder _tokenEncoder;
        private readonly IJsonWebAlgorithm _jsonWebAlgorithm;
        private readonly ITokenData _tokenData;
        private readonly IApiHelper _apiHelper;

        public DfESignInService(
            IOptions<DfEOidcConfiguration> configuration,
            ITokenDataSerializer tokenDataSerializer,
            ITokenEncoder tokenEncoder,
            IJsonWebAlgorithm jsonWebAlgorithm,
            ITokenData tokenData,
            IApiHelper apiHelper)
        {
            _configuration = configuration.Value;
            _tokenDataSerializer = tokenDataSerializer;
            _tokenEncoder = tokenEncoder;
            _jsonWebAlgorithm = jsonWebAlgorithm;
            _tokenData = tokenData;
            _apiHelper = apiHelper;
        }

        public async Task PopulateAccountClaims(TokenValidatedContext ctx)
        {
            #region Argument Validation
            if(ctx is null) throw new ArgumentNullException(nameof(ctx));
            #endregion

            var userOrganisation = JsonConvert.DeserializeObject<Organisation>
            (
                ctx.Principal.GetClaimValue(ClaimName.Organisation)
            );

            if (userOrganisation != null && ctx.Principal != null)
            {
                var userId = ctx.Principal.GetClaimValue(ClaimName.Sub);
                var ukPrn = Convert.ToString(userOrganisation.UkPrn) ?? string.Empty;

#if DEBUG
                // strictly only for development purpose
                // populating the UkPrn with dummy value. Make sure this line is commented out before commiting the changes.

                //ukPrn = string.IsNullOrEmpty(ukPrn) ? "10000001" : ukPrn; 
#endif

                if (userId != null)
                    await PopulateDfEClaims(ctx, userId, Convert.ToString(userOrganisation.Id));

                var displayName = $"{ctx.Principal.GetClaimValue(ClaimName.GivenName)} {ctx.Principal.GetClaimValue(ClaimName.FamilyName)}";

                ctx.HttpContext.Items.Add(ClaimsIdentity.DefaultNameClaimType, ukPrn);
                ctx.HttpContext.Items.Add(CustomClaimsIdentity.DisplayName, displayName);

                ctx.Principal.Identities.First().AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, ukPrn));
                ctx.Principal.Identities.First().AddClaim(new Claim(CustomClaimsIdentity.DisplayName, displayName));
                ctx.Principal.Identities.First().AddClaim(new Claim(CustomClaimsIdentity.UkPrn, ukPrn));
                ctx.Principal.Identities.First().AddClaim(new Claim(CustomClaimsIdentity.Service, "DAA"));
            }
        }

        public async Task PopulateDfEClaims(TokenValidatedContext ctx, string userId, string userOrgId)
        {
            _apiHelper.AccessToken = CreateToken();
            var destinationUrl = GetDestinationUrl(userId, userOrgId);
            var response = await _apiHelper.Get<ApiServiceResponse>(destinationUrl);

            if (response != null)
            {
                var roleClaims = new List<Claim>();
                foreach (var role in response.Roles.Where(role => role.Status.Id.Equals(1)))
                {
                    roleClaims.Add(new Claim(ClaimName.RoleCode, role.Code, ClaimTypes.Role, ctx.Options.ClientId));
                    roleClaims.Add(new Claim(ClaimName.RoleId, role.Id.ToString(), ClaimTypes.Role, ctx.Options.ClientId));
                    roleClaims.Add(new Claim(ClaimName.RoleName, role.Name, ClaimTypes.Role, ctx.Options.ClientId));
                    roleClaims.Add(new Claim(ClaimName.RoleNumericId, role.NumericId.ToString(), ClaimTypes.Role, ctx.Options.ClientId));

                    // Add to initial identity
                    ctx.Principal?.Identities.First()
                        .AddClaim(new Claim(CustomClaimsIdentity.Service, role.Name));
                }

                ctx?.Principal?.AddIdentity(new ClaimsIdentity(roleClaims));
            }
        }

        /// <summary>
        /// Method to generate the bearer token.
        /// </summary>
        /// <returns>string.</returns>
        private string CreateToken()
        {
            _tokenData.Header.Add("typ", AuthConfig.TokenType);

            return new TokenBuilder(_tokenDataSerializer, _tokenData, _tokenEncoder, _jsonWebAlgorithm)
                .UseAlgorithm(AuthConfig.Algorithm)
                .ForAudience(AuthConfig.Aud)
                .WithSecretKey(_configuration.APIServiceSecret)
                .Issuer(_configuration.ClientId)
                .CreateToken();
        }

        /// <summary>
        /// Method to generate the destination/endpoint url.
        /// </summary>
        /// <param name="userId">User Identifier.</param>
        /// <param name="orgId">Organisation Identifier.</param>
        /// <returns>string.</returns>
        /// <exception cref="MemberAccessException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        internal string GetDestinationUrl(string userId, string orgId)
        {
            #region Check Members
            if(string.IsNullOrEmpty(_configuration.APIServiceId)) throw new MemberAccessException(nameof(_configuration.APIServiceId));
            if(string.IsNullOrEmpty(_configuration.APIServiceUrl)) throw new MemberAccessException(nameof(_configuration.APIServiceUrl));
            if(string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));
            if(string.IsNullOrEmpty(orgId)) throw new ArgumentNullException(nameof(orgId));
            #endregion

            return $"{_configuration.APIServiceUrl}/services/{_configuration.APIServiceId}/organisations/{orgId}/users/{userId}";
        }
    }
}