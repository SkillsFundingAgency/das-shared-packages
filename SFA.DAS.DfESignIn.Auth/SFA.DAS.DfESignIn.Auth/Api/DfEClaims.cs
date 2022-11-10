using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Api.Models;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.Api.Client
{
    public class DfEClaims : IDfEClaims
    {
        public async Task GetClaims(TokenValidatedContext ctx, string userId, string userOrgId, IConfiguration config)
        {
            var clientFactory = new DfESignInClientFactory(config);
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