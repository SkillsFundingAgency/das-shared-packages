using System;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Moq;

namespace SFA.DAS.OidcMiddleware.UnitTests
{
    public abstract class OwinAuthTestBase
    {
        protected Mock<IOwinResponse> OwinResponse;
        protected Mock<IOwinRequest> OwinRequest;
        protected Mock<IOwinContext> OwinContext;
        protected Mock<IAuthenticationManager> AuthenticationManager;
        public abstract string RequestUrl { get; set; }

        protected void ArrangeBase()
        {
            OwinResponse = new Mock<IOwinResponse>();
            OwinResponse.Setup(x => x.StatusCode).Returns(401);

            OwinRequest = new Mock<IOwinRequest>();
            OwinRequest.Setup(r => r.Uri).Returns(new Uri(RequestUrl));

            AuthenticationManager = new Mock<IAuthenticationManager>();

            AuthenticationManager.Setup(x => x.AuthenticateAsync("TempState")).ReturnsAsync(new AuthenticateResult(new ClaimsIdentity(),new AuthenticationProperties(), new AuthenticationDescription()));
            AuthenticationManager.Setup(x => x.SignOut("TempState"));

            OwinContext = new Mock<IOwinContext>();
            OwinContext.Setup(x => x.Request).Returns(OwinRequest.Object);
            OwinContext.Setup(x => x.Response).Returns(OwinResponse.Object);
            OwinContext.Setup(x => x.Authentication).Returns(AuthenticationManager.Object);

        }
    }
}