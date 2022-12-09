using System.Security.Claims;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.DfESignIn.Auth.Api.Models;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Constants;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.DfESignIn.Auth.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Services
{
    public class DfESignInServiceTest
    {
        [Test, MoqAutoData]
        public async Task Then_The_Claims_Are_Populated_For_Users(
            string userId,
            Organisation organisation,
            DfEOidcConfiguration config,
            ApiServiceResponse response,
            [Frozen] Mock<IApiHelper> apiHelper,
            [Frozen] Mock<IOptions<DfEOidcConfiguration>> configuration)
        {
            var tokenValidatedContext = ArrangeTokenValidatedContext(userId, organisation, string.Empty);
            apiHelper.Setup(x => x.Get<ApiServiceResponse>($"{config.APIServiceUrl}/services/{config.APIServiceId}/organisations/{organisation.Id}/users/{userId}")).ReturnsAsync(response);
            configuration.Setup(c => c.Value).Returns(config);
            var service = new DfESignInService(configuration.Object, apiHelper.Object);
                
            await service.PopulateAccountClaims(tokenValidatedContext);

            var actualClaims = tokenValidatedContext.Principal?.Identities.First().Claims.ToList();
            actualClaims?.First(c => c.Type.Equals(CustomClaimsIdentity.UkPrn)).Value.Should().Be(organisation.UkPrn.ToString());
            actualClaims?.First(c => c.Type.Equals(ClaimsIdentity.DefaultNameClaimType)).Value.Should().Be(userId);
            actualClaims?.First(c => c.Type.Equals(ClaimName.Sub)).Value.Should().Be(userId);
            actualClaims?.First(c => c.Type.Equals(CustomClaimsIdentity.DisplayName)).Value.Should().Be("Test Tester");
        }
        
        [Test, MoqAutoData]
        public async Task Then_The_Organisations_Are_Added_To_The_Claims(
            string userId,
            Organisation organisation,
            DfEOidcConfiguration config,
            ApiServiceResponse response,
            [Frozen] Mock<IApiHelper> apiHelper,
            [Frozen] Mock<IOptions<DfEOidcConfiguration>> configuration)
        {
            var tokenValidatedContext = ArrangeTokenValidatedContext(userId, organisation, string.Empty);
            response.Roles = response.Roles.Select(c => {c.Status.Id = 1; return c;}).ToList();
            apiHelper.Setup(x => x.Get<ApiServiceResponse>($"{config.APIServiceUrl}/services/{config.APIServiceId}/organisations/{organisation.Id}/users/{userId}")).ReturnsAsync(response);
            configuration.Setup(c => c.Value).Returns(config);
            var service = new DfESignInService(configuration.Object, apiHelper.Object);
                
            await service.PopulateAccountClaims(tokenValidatedContext);

            var claims = tokenValidatedContext.Principal?.Identities.Last().Claims.ToList();
            if (claims != null)
            {

                claims.Where(x => x.Type.Equals(ClaimName.RoleCode)).Select(c => c.Value).Should()
                    .BeEquivalentTo(response.Roles.Select(c => c.Code).ToList());
                claims.Where(x => x.Type.Equals(ClaimName.RoleId)).Select(c => c.Value).Should()
                    .BeEquivalentTo(response.Roles.Select(c => c.Id.ToString()).ToList());
                claims.Where(x => x.Type.Equals(ClaimName.RoleName)).Select(c => c.Value).Should()
                    .BeEquivalentTo(response.Roles.Select(c => c.Name).ToList());
                claims.Where(x => x.Type.Equals(ClaimName.RoleNumericId)).Select(c => c.Value).Should()
                    .BeEquivalentTo(response.Roles.Select(c => c.NumericId.ToString()).ToList());
            }
                
        }
        
        private TokenValidatedContext ArrangeTokenValidatedContext(string userId, Organisation organisation, string emailAddress)
        {
            var identity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimName.Sub, userId),
                new Claim(ClaimTypes.Email, emailAddress),
                new Claim(ClaimName.Organisation, JsonConvert.SerializeObject(organisation)),
                new Claim(ClaimName.FamilyName, "Tester"),
                new Claim(ClaimName.GivenName, "Test")
            });
        
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(identity));
            return new TokenValidatedContext(new DefaultHttpContext(), new AuthenticationScheme(",","", typeof(TestAuthHandler)),
                new OpenIdConnectOptions(), Mock.Of<ClaimsPrincipal>(), new AuthenticationProperties())
            {
                Principal = claimsPrincipal
            };
        }
    
    
        private class TestAuthHandler : IAuthenticationHandler
        {
            public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
            {
                throw new NotImplementedException();
            }

            public Task<AuthenticateResult> AuthenticateAsync()
            {
                throw new NotImplementedException();
            }

            public Task ChallengeAsync(AuthenticationProperties? properties)
            {
                throw new NotImplementedException();
            }

            public Task ForbidAsync(AuthenticationProperties? properties)
            {
                throw new NotImplementedException();
            }
        }
    }
}
