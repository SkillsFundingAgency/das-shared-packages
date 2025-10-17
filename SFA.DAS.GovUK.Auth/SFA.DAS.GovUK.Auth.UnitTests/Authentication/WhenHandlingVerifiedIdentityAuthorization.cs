using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.GovUK.Auth.Authentication;

namespace SFA.DAS.GovUK.Auth.UnitTests.Authentication
{
    [TestFixture]
    public class WhenHandlingVerifiedIdentityAuthorization
    {
        private VerifiedIdentityAuthorizationHandler _handler;
        private VerifiedIdentityRequirement _requirement;

        [SetUp]
        public void SetUp()
        {
            _handler = new VerifiedIdentityAuthorizationHandler();
            _requirement = new VerifiedIdentityRequirement();
        }

        [Test]
        public async Task Then_If_Vot_Contains_P2_Then_Succeed()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("vot", "Cl.Cm.P2")
            }));

            var authContext = new AuthorizationHandlerContext(
                requirements: new[] { _requirement },
                user: context.User,
                resource: context);

            await _handler.HandleAsync(authContext);

            authContext.HasSucceeded.Should().BeTrue();
        }

        [Test]
        public async Task Then_If_Vot_Missing_Or_Does_Not_Contain_P2_Then_Fail_And_Redirect()
        {
            var context = new DefaultHttpContext();
            context.Request.Path = "/secure/resource";
            context.Request.QueryString = new QueryString("?x=1");

            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("vot", "Cl.Cm") // No P2
            }));

            var authContext = new AuthorizationHandlerContext(
                requirements: new[] { _requirement },
                user: context.User,
                resource: context);

            await _handler.HandleAsync(authContext);

            authContext.HasFailed.Should().BeTrue();
            context.Response.Headers["Location"].ToString().Should()
                .Be("/service/verify-identity?returnUrl=%2Fsecure%2Fresource%3Fx%3D1");
        }

        [Test]
        public async Task Then_If_Resource_Is_AuthorizationFilterContext_And_Vot_Valid_Then_Succeed()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("vot", "Cl.Cm.P2")
            }));

            var filterContext = new AuthorizationFilterContext(
                new ActionContext(
                    httpContext,
                    new Microsoft.AspNetCore.Routing.RouteData(),
                    new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
                ),
                Enumerable.Empty<IFilterMetadata>().ToList());

            var authContext = new AuthorizationHandlerContext(
                new[] { _requirement },
                httpContext.User,
                filterContext);

            await _handler.HandleAsync(authContext);

            authContext.HasSucceeded.Should().BeTrue();
        }
    }
}
