using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.GovUK.Auth.Authentication;

namespace SFA.DAS.GovUK.Auth.UnitTests.Authentication;

[TestFixture]
public class AccountActiveAuthorizationHandlerTests
{
    private AccountActiveAuthorizationHandler _handler;
    private AccountActiveRequirement _requirement;

    [SetUp]
    public void SetUp()
    {
        _handler = new AccountActiveAuthorizationHandler();
        _requirement = new AccountActiveRequirement();
    }

    [Test]
    public async Task Then_If_Claim_Is_Missing_Then_Succeeds()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        var context = new AuthorizationHandlerContext([_requirement], user, resource: null);

        await _handler.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Test]
    public async Task Then_If_Claim_Is_Not_Suspended_Then_Succeeds()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.AuthorizationDecision, "Active")]));
        var context = new AuthorizationHandlerContext([_requirement], user, resource: null);

        await _handler.HandleAsync(context);

        context.HasSucceeded.Should().BeTrue();
    }

    [Test]
    public async Task Then_If_Claim_Is_Suspended_Then_Fails()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.AuthorizationDecision, AuthorizationDecisions.Suspended)]));
        var context = new AuthorizationHandlerContext([_requirement], user, resource: null);

        await _handler.HandleAsync(context);

        context.HasFailed.Should().BeTrue();
    }
}
