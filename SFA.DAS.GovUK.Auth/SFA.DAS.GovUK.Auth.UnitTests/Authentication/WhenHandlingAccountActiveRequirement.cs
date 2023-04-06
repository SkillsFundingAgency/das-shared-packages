using System.Security.Claims;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Moq;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.GovUK.Auth.UnitTests.Authentication;

public class WhenHandlingAccountActiveRequirement
{
    [Test, MoqAutoData]
    public async Task Then_If_Claim_Does_Not_Exist_Then_Succeeds(
        string role,
        AccountActiveRequirement requirement,
        AccountActiveAuthorizationHandler authorizationHandler)
    {
        //Arrange
        var httpContextBase = new Mock<HttpContext>();
        var response = new Mock<HttpResponse>();
        httpContextBase.Setup(c => c.Response).Returns(response.Object);
        var claim = new Claim("AccountSuspended", "true");
        var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[] {claim})});
        var context = new AuthorizationHandlerContext(new [] {requirement}, claimsPrinciple, httpContextBase.Object);
            
        //Act
        await authorizationHandler.HandleAsync(context);

        //Assert
        context.HasSucceeded.Should().BeTrue();
        response.Verify(x=>x.Redirect("/Errors/AccountSuspended"), Times.Never);
    }
    
    [Test, MoqAutoData]
    public async Task Then_If_Claim_Does_Exist_And_Not_Suspended_Then_Succeeds(
        string role,
        AccountActiveRequirement requirement,
        AccountActiveAuthorizationHandler authorizationHandler)
    {
        //Arrange
        var httpContextBase = new Mock<HttpContext>();
        var response = new Mock<HttpResponse>();
        httpContextBase.Setup(c => c.Response).Returns(response.Object);
        var claim = new Claim(ClaimTypes.AuthorizationDecision, "active");
        var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[] {claim})});
        var context = new AuthorizationHandlerContext(new [] {requirement}, claimsPrinciple, httpContextBase.Object);
            
        //Act
        await authorizationHandler.HandleAsync(context);

        //Assert
        context.HasSucceeded.Should().BeTrue();
        response.Verify(x=>x.Redirect("/Errors/AccountSuspended"), Times.Never);
    }
    
    [Test, MoqAutoData]
    public async Task Then_If_Claim_Does_Exist_And_Is_Suspended_Then_Succeeds_And_Redirects(
        string role,
        [Frozen] Mock<IConfiguration> configuration,
        AccountActiveRequirement requirement,
        AccountActiveAuthorizationHandler authorizationHandler)
    {
        //Arrange
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("INT");
        var httpContextBase = new Mock<HttpContext>();
        var response = new Mock<HttpResponse>();
        httpContextBase.Setup(c => c.Response).Returns(response.Object);
        var claim = new Claim(ClaimTypes.AuthorizationDecision, "sUsPended");
        var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[] {claim})});
        var context = new AuthorizationHandlerContext(new [] {requirement}, claimsPrinciple, httpContextBase.Object);
            
        //Act
        await authorizationHandler.HandleAsync(context);

        //Assert
        context.HasSucceeded.Should().BeTrue();
        response.Verify(x=>x.Redirect("https://employerprofiles.int-eas.apprenticeships.education.gov.uk/service/account-unavailable"));
    }
    
     
    [Test, MoqAutoData]
    public async Task Then_If_Claim_Does_Exist_And_Is_Suspended_Then_Succeeds_And_Redirects_For_Filter_Context(
        string role,
        [Frozen] Mock<IConfiguration> configuration,
        AccountActiveRequirement requirement,
        AccountActiveAuthorizationHandler authorizationHandler)
    {
        //Arrange
        configuration.Setup(x => x["ResourceEnvironmentName"]).Returns("INT");
        
        
        var httpContextBase = new Mock<HttpContext>();
        var response = new Mock<HttpResponse>();
        httpContextBase.Setup(c => c.Response).Returns(response.Object);
        var filterContext = new AuthorizationFilterContext(new ActionContext(httpContextBase.Object, new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>());
        var claim = new Claim(ClaimTypes.AuthorizationDecision, "sUsPended");
        var claimsPrinciple = new ClaimsPrincipal(new[] {new ClaimsIdentity(new[] {claim})});
        var context = new AuthorizationHandlerContext(new [] {requirement}, claimsPrinciple, filterContext);
            
        //Act
        await authorizationHandler.HandleAsync(context);

        //Assert
        context.HasSucceeded.Should().BeTrue();
        response.Verify(x=>x.Redirect("https://employerprofiles.int-eas.apprenticeships.education.gov.uk/service/account-unavailable"));
    }
}