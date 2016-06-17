using System;
using System.Security.Claims;
using Moq;
using NUnit.Framework;
using SFA.DAS.OidcMiddleware.Builders;
using SFA.DAS.OidcMiddleware.Strategies;

namespace SFA.DAS.OidcMiddleware.UnitTests.ApplyResponseChallengeStrategyTests
{
    public class WhenApplyingResponseChallenge : OwinAuthTestBase
    {
        private const string AuthorizeEndpoint = "http://localhost";
        private const string ClientId = "TheClientId";
        private const string Scopes = "RequestedScopes";
        private const string AuthorizationRedirectUrl = "http://idp.test";

        private ApplyResponseChallengeStrategy _applyResponseChallengeStrategy;
        private Mock<IBuildAuthorizeRequestUrl> _buildAuthorizeRequestUrl;
        public override string RequestUrl { get; set; }

        [SetUp]
        public void Arrange()
        {
            RequestUrl = "http://unit.tests";
            ArrangeBase();

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
            OwinResponse.Setup(x => x.StatusCode).Returns(200);

            //Act
            _applyResponseChallengeStrategy.ApplyResponseChallenge(OwinContext.Object);

            //Assert
            Assert.AreEqual(200, OwinResponse.Object.StatusCode);
            _buildAuthorizeRequestUrl.Verify(x=>x.GetAuthorizeRequestUrl(It.IsAny<string>(),It.IsAny<Uri>(),It.IsAny<string>(),It.IsAny<string>(),It.IsAny<string>(), It.IsAny<string>()),Times.Never);
        }

        [Test]
        public void ThenItShouldRedirectResponseToTheAuthorizationUrlIfTheStatusCodeIs401()
        {
            //Act
            _applyResponseChallengeStrategy.ApplyResponseChallenge(OwinContext.Object);

            //Assert
            OwinResponse.Verify(r => r.Redirect(AuthorizationRedirectUrl));
        }

        [Test]
        public void ThenTheNonceAndStateAreAddedToTheClaims()
        {
            //Act
            _applyResponseChallengeStrategy.ApplyResponseChallenge(OwinContext.Object);

            //Assert
            AuthenticationManager.Verify(x => x.SignIn(It.Is<ClaimsIdentity>(i => i.HasClaim(c => c.Type == "nonce"))));
            AuthenticationManager.Verify(x => x.SignIn(It.Is<ClaimsIdentity>(i => i.HasClaim(c => c.Type == "state"))));
        }
        
    }
}
