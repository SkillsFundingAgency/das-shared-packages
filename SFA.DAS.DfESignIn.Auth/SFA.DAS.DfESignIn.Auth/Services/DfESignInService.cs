using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Api.Client;
using SFA.DAS.DfESignIn.Auth.Api.Models;
using SFA.DAS.DfESignIn.Auth.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SFA.DAS.DfESignIn.Auth.UnitTests")]

namespace SFA.DAS.DfESignIn.Auth.Services
{
    internal class DfESignInService : IDfESignInService
    {
        private readonly HttpClient _httpClient;
        private readonly DfEOidcConfiguration _configuration;
        private readonly ITokenDataSerializer _tokenDataSerializer;
        private readonly ITokenEncoder _tokenEncoder;
        private readonly IJsonWebAlgorithm _jsonWebAlgorithm;
        private readonly ITokenData _tokenData;

        public DfESignInService(
            HttpClient httpClient,
            IOptions<DfEOidcConfiguration> configuration,
            ITokenDataSerializer tokenDataSerializer,
            ITokenEncoder tokenEncoder,
            IJsonWebAlgorithm jsonWebAlgorithm,
            ITokenData tokenData
            )
        {
            _httpClient = httpClient;
            _configuration = configuration.Value;
            _tokenDataSerializer = tokenDataSerializer;
            _tokenEncoder = tokenEncoder;
            _jsonWebAlgorithm = jsonWebAlgorithm;
            _tokenData = tokenData;
        }

        public async Task PopulateAccountClaims(TokenValidatedContext ctx)
        {
            var userId = ctx.Principal.Claims.Where(c => c.Type.Contains("sub")).Select(c => c.Value).SingleOrDefault();

            var userOrganisation = JsonConvert.DeserializeObject<Organisation>
            (
                ctx.Principal.Claims.Where(c => c.Type == "organisation")
                .Select(c => c.Value)
                .FirstOrDefault()
            );
            var ukPrn = userOrganisation.UkPrn;
            
            if (userId != null && userOrganisation?.Id != null)
                await PopulateDfEClaims(ctx, userId, userOrganisation.Id.ToString());

            var displayName = ctx.Principal.Claims.FirstOrDefault(c => c.Type.Equals("given_name")).Value + " " + ctx.Principal.Claims.FirstOrDefault(c => c.Type.Equals("family_name")).Value;
            ctx.HttpContext.Items.Add(ClaimsIdentity.DefaultNameClaimType, ukPrn.ToString());
            ctx.HttpContext.Items.Add("http://schemas.portal.com/displayname", displayName);
            ctx.Principal.Identities.First().AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, ukPrn.ToString()));
            ctx.Principal.Identities.First().AddClaim(new Claim("http://schemas.portal.com/displayname", displayName));
            ctx.Principal.Identities.First().AddClaim(new Claim("http://schemas.portal.com/ukprn", ukPrn.ToString()));
            ctx.Principal.Identities.First().AddClaim(new Claim("http://schemas.portal.com/service", "DAA"));
        }

        public async Task PopulateDfEClaims(TokenValidatedContext ctx, string userId, string userOrgId)
        {
            var clientFactory = new DfESignInClientFactory(_configuration, _httpClient, _tokenDataSerializer, _tokenEncoder, _jsonWebAlgorithm, _tokenData);
            DfESignInClient dfeSignInClient = clientFactory.CreateDfESignInClient(userId, userOrgId);
            HttpResponseMessage response = await dfeSignInClient.HttpClient.GetAsync(dfeSignInClient.TargetAddress);

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStringAsync();

                var apiServiceResponse = JsonConvert.DeserializeObject<ApiServiceResponse>(stream);
                var roleClaims = new List<Claim>();
                foreach (var role in apiServiceResponse.Roles)
                {
                    if (role.Status.Id.Equals(1))
                    {
                        roleClaims.Add(new Claim("rolecode", role.Code, ClaimTypes.Role, ctx.Options.ClientId));
                        roleClaims.Add(new Claim("roleId", role.Id.ToString(), ClaimTypes.Role, ctx.Options.ClientId));
                        roleClaims.Add(new Claim("roleName", role.Name, ClaimTypes.Role, ctx.Options.ClientId));
                        roleClaims.Add(new Claim("rolenumericid", role.NumericId.ToString(), ClaimTypes.Role, ctx.Options.ClientId));
                    }
                }

                var roleIdentity = new ClaimsIdentity(roleClaims);
                ctx.Principal.AddIdentity(roleIdentity);
            }
        }
    }
}