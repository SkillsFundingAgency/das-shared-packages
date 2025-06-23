using System.Security.Claims;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.GovUK.Auth.UnitTests.Services;

public class StubAuthenticationServiceTests
{
    [Test, MoqAutoData]
    public async Task GetStubSignInClaims_Then_The_Claims_Are_Added_From_The_Model(
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<ICustomClaims> claims)
    {
        httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
        
        claims.Setup(x => x.GetClaims(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new List<Claim> ());
        var service = new StubAuthenticationService(configuration.Object, claims.Object, httpContextAccessor.Object);
        
        var actual = await service.GetStubSignInClaims(new StubAuthUserDetails
        {
            Id = "12345",
            Email = "someemail@somewhere.com"
        });

        actual!.Identity!.AuthenticationType.Should().Be(CookieAuthenticationDefaults.AuthenticationScheme);
        actual.Identities.FirstOrDefault()!.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))!.Value.Should()
            .Be("someemail@somewhere.com");
        actual.Identities.FirstOrDefault()!.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier))!.Value.Should()
            .Be("12345");
        actual.Identities.FirstOrDefault()!.Claims.FirstOrDefault(c => c.Type.Equals("sub"))!.Value.Should()
            .Be("12345");
    }

    [Test, MoqAutoData]
    public async Task GetStubSignInClaims_Then_The_Custom_Clams_Are_Added(
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        [Frozen] Mock<IConfiguration> configuration,
        [Frozen] Mock<ICustomClaims> claims)
    {
        httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
        var claim = new Claim("CustomClaimKey", "CustomClaimValue");
        claims.Setup(x => x.GetClaims(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new List<Claim> {claim});
        var service = new StubAuthenticationService(configuration.Object, claims.Object, httpContextAccessor.Object);
        
        var actual = await service.GetStubSignInClaims(new StubAuthUserDetails
        {
            Id = "12345",
            Email = "someemail@somewhere.com"
        });

        actual.Identities.FirstOrDefault()!.AuthenticationType.Should().Be(CookieAuthenticationDefaults.AuthenticationScheme);
        actual.Identities.FirstOrDefault()!.Claims.FirstOrDefault(c => c.Type.Equals("CustomClaimKey"))!.Value.Should()
            .Be("CustomClaimValue");
    }

    [Test, MoqAutoData]
    public async Task GetStubSignInClaims_Then_Null_Is_Returned_If_Prod(
        [Frozen] Mock<IConfiguration> configuration)
    {
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("prd");
        var service = new StubAuthenticationService(configuration.Object, null, null);
        
        var actual = await service.GetStubSignInClaims(new StubAuthUserDetails());

        actual.Should().BeNull();
    }
}