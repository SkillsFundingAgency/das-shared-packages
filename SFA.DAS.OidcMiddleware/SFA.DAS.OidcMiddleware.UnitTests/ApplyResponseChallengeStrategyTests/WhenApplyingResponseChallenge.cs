using System;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.OidcMiddleware.UnitTests.ApplyResponseChallengeStrategyTests
{
    public class WhenApplyingResponseChallenge
    {
        private const string RequestUrl = "http://unit.tests";
        private const string AuthorizeEndpoint = "http://localhost";
        private const string ClientId = "TheClientId";
        private const string Scopes = "RequestedScopes";
        private const string AuthorizationRedirectUrl = "http://idp.test";

        private ApplyResponseChallengeStrategy _applyResponseChallengeStrategy;
        private Mock<IOwinResponse> _owinResponse;
        private Mock<IBuildAuthorizeRequestUrl> _buildAuthorizeRequestUrl;
        private Mock<IOwinRequest> _owinRequest;
        private Mock<IOwinContext> _owinContext;
        private Mock<IAuthenticationManager> _authenticationManager;

        [SetUp]
        public void Arrange()
        {
            _owinResponse = new Mock<IOwinResponse>();
            _owinResponse.Setup(x => x.StatusCode).Returns(401);

            _owinRequest = new Mock<IOwinRequest>();
            _owinRequest.Setup(r => r.Uri).Returns(new Uri(RequestUrl));

            _authenticationManager = new Mock<IAuthenticationManager>();

            _owinContext = new Mock<IOwinContext>();
            _owinContext.Setup(x => x.Request).Returns(_owinRequest.Object);
            _owinContext.Setup(x => x.Response).Returns(_owinResponse.Object);
            _owinContext.Setup(x => x.Authentication).Returns(_authenticationManager.Object);

            _buildAuthorizeRequestUrl = new Mock<IBuildAuthorizeRequestUrl>();
            _buildAuthorizeRequestUrl.Setup(b => b.GetAuthorizeRequestUrl(AuthorizeEndpoint, It.Is<Uri>(u => u.OriginalString == RequestUrl), ClientId, Scopes, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(AuthorizationRedirectUrl);
            
            _applyResponseChallengeStrategy = new ApplyResponseChallengeStrategy(new OidcMiddlewareOptions("")
            {
                AuthorizeEndpoint = AuthorizeEndpoint,
                ClientId = ClientId,
                Scopes = Scopes
            }, _buildAuthorizeRequestUrl.Object);
        }



        [Test]
        public void ThenItShouldNotProcessAResponseThatDoesNotHaveStatus401()
        {
            //Arrange
            _owinResponse.Setup(x => x.StatusCode).Returns(200);

            //Act
            _applyResponseChallengeStrategy.ApplyResponseChallenge(_owinContext.Object);

            //Assert
            Assert.AreEqual(200, _owinResponse.Object.StatusCode);
            _buildAuthorizeRequestUrl.Verify(x=>x.GetAuthorizeRequestUrl(It.IsAny<string>(),It.IsAny<Uri>(),It.IsAny<string>(),It.IsAny<string>(),It.IsAny<string>(), It.IsAny<string>()),Times.Never);
        }

        [Test]
        public void ThenItShouldRedirectResponseToTheAuthorizationUrlIfTheStatusCodeIs401()
        {
            //Act
            _applyResponseChallengeStrategy.ApplyResponseChallenge(_owinContext.Object);

            //Assert
            _owinResponse.Verify(r => r.Redirect(AuthorizationRedirectUrl));
        }

        [Test]
        public void ThenTheNonceAndStateAreAddedToTheClaims()
        {
            //Act
            _applyResponseChallengeStrategy.ApplyResponseChallenge(_owinContext.Object);

            //Assert
            _authenticationManager.Verify(x => x.SignIn(It.Is<ClaimsIdentity>(i => i.HasClaim(c => c.Type == "nonce"))));
            _authenticationManager.Verify(x => x.SignIn(It.Is<ClaimsIdentity>(i => i.HasClaim(c => c.Type == "state"))));
        }
    }
}
