using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Moq;
using NUnit.Framework;
using SFA.DAS.OidcMiddleware.Builders;
using SFA.DAS.OidcMiddleware.Strategies;
using SFA.DAS.OidcMiddleware.Validators;

namespace SFA.DAS.OidcMiddleware.UnitTests.AuthenticateCoreStrategyTests
{
    public class WhenAuthenticating : OwinAuthTestBase
    {
        private const string State = "546c8bcae027494ebfca4743a37b8816";
        private const string Nonce = "somevalue";
        private const string CodeQueryValue = "73NRxkq63dDxd1NR";
        private const string ClientId = "someclientId";
        private const string ClientSecret = "mysecret";
        private const string AccessToken = "myaccesstoken";
        private const string RefreshToken = "myrefreshtoken";
        private const string TokenEndpoint = "http://localtoken.endpoint";
        private const string IdentityToken = "12345";
        private const string LoggedInName = "logged in name";
        public override string RequestUrl { get; set; }

        private AuthenticateCoreStrategy _authenticateCoreStrategy;
        private Mock<ISecureDataFormat<AuthenticationProperties>> _secureDataFormat;
        private Mock<IBuildRequestAuthorisationCode> _buildReqeustAuthorisationCode;
        private Mock<IBuildUserInfoClientUrl> _buildUserInfoClientUrl;
        private OidcMiddlewareOptions _oidcMiddlewareOptions;
        private Mock<ISecurityTokenValidation> _securityTokenValidation;

        [SetUp]
        public void Arrange()
        {
            RequestUrl = "http://localhost.test/some/test?somequeryValue=123456";
            ArrangeBase();
            
            AuthenticationManager.Setup(x => x.AuthenticateAsync("TempState")).ReturnsAsync(
                new AuthenticateResult(new ClaimsIdentity(new List<Claim> { new Claim("nonce", Nonce), new Claim("state", State) }),
                new AuthenticationProperties(),
                new AuthenticationDescription()));

            var readableStringcollection = new Mock<IReadableStringCollection>();
            readableStringcollection.Setup(x => x.GetValues("state")).Returns(new List<string> { State });
            readableStringcollection.Setup(x => x.GetValues("code")).Returns(new List<string> { CodeQueryValue });

            OwinRequest.Setup(x => x.Query).Returns(readableStringcollection.Object);
            OwinContext.Setup(x => x.Authentication).Returns(AuthenticationManager.Object);

            _secureDataFormat = new Mock<ISecureDataFormat<AuthenticationProperties>>();
            _secureDataFormat.Setup(x => x.Unprotect(State)).Returns(new AuthenticationProperties(new Dictionary<string, string> { { "SomeKey", "SomeValue" } }));

            _buildReqeustAuthorisationCode = new Mock<IBuildRequestAuthorisationCode>();
            var tokenResponse = new TokenResponse("{\"id_token\": \""+ IdentityToken + "\",\"access_token\": \"" + AccessToken + "\",\"refresh_token\": \"" + RefreshToken + "\"}");
            _buildReqeustAuthorisationCode.Setup(x => x.GetTokenResponse(TokenEndpoint, ClientId, ClientSecret, CodeQueryValue, new Uri(RequestUrl))).ReturnsAsync(tokenResponse);


            _securityTokenValidation = new Mock<ISecurityTokenValidation>();
            _securityTokenValidation.Setup(x => x.ValidateToken(It.IsAny<string>(), It.IsAny<string>())).Returns(new List<Claim>());

            _buildUserInfoClientUrl = new Mock<IBuildUserInfoClientUrl>();
            

            _oidcMiddlewareOptions = new OidcMiddlewareOptions("data")
            {
                StateDataFormat = _secureDataFormat.Object,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                TokenEndpoint = TokenEndpoint
            };
            _buildUserInfoClientUrl.Setup(x => x.GetUserClaims(_oidcMiddlewareOptions, AccessToken)).ReturnsAsync(new List<Claim> {new Claim("name",LoggedInName)});

            _authenticateCoreStrategy = new AuthenticateCoreStrategy(_oidcMiddlewareOptions, _buildReqeustAuthorisationCode.Object, _buildUserInfoClientUrl.Object, _securityTokenValidation.Object);
        }

        [Test]
        public async Task ThenAnEmtpyAuthenticationTicketIsReturnedWhenTheQueryHasNoCodeQueryStringParameter()
        {
            //Arrange
            var readableStringcollection = new Mock<IReadableStringCollection>();
            readableStringcollection.Setup(x => x.GetValues("state")).Returns(new List<string> { State });
            OwinRequest.Setup(x => x.Query).Returns(readableStringcollection.Object);

            //Act
            var actual = await _authenticateCoreStrategy.Authenticate(OwinContext.Object);

            //Assert
            Assert.IsAssignableFrom<AuthenticationTicket>(actual);
            Assert.IsNull(actual.Identity);
            Assert.IsEmpty(actual.Properties.Dictionary);
        }

        [Test]
        public async Task ThenAnEmtpyAuthenticationTicketIsReturnedWhenTheQueryHasNoStateQueryStringParameter()
        {
            //Arrange
            var readableStringcollection = new Mock<IReadableStringCollection>();
            readableStringcollection.Setup(x => x.GetValues("code")).Returns(new List<string> { CodeQueryValue });
            OwinRequest.Setup(x => x.Query).Returns(readableStringcollection.Object);

            //Act
            var actual = await _authenticateCoreStrategy.Authenticate(OwinContext.Object);

            //Assert
            Assert.IsAssignableFrom<AuthenticationTicket>(actual);
            Assert.IsNull(actual.Identity);
            Assert.IsEmpty(actual.Properties.Dictionary);
        }

        [Test]
        public async Task ThenTheStateIsCorrectlyReadFromTheQueryStringAndUnprotectedAndReturned()
        {
            //Act
            var actual = await _authenticateCoreStrategy.Authenticate(OwinContext.Object);

            //Assert
            _secureDataFormat.Verify(x => x.Unprotect(State), Times.Once);
            Assert.IsNotEmpty(actual.Properties.Dictionary);

        }

        [Test]
        public async Task ThenTheTokenResponseIsRetrievedUsingTheCode()
        {
            //Act
            await _authenticateCoreStrategy.Authenticate(OwinContext.Object);

            //Assert
            _buildReqeustAuthorisationCode.Verify(x => x.GetTokenResponse(TokenEndpoint, ClientId, ClientSecret, CodeQueryValue, new Uri(RequestUrl)));
        }

        [Test]
        public async Task ThenTheTempStateIsReadFromTheAuthenticatedUser()
        {
            //Act
            await _authenticateCoreStrategy.Authenticate(OwinContext.Object);

            //Assert
            AuthenticationManager.Verify(x => x.AuthenticateAsync("TempState"), Times.Once);
        }

        [Test]
        public async Task ThenTheTempStateIsSignedOutFrom()
        {
            //Act
            await _authenticateCoreStrategy.Authenticate(OwinContext.Object);

            //Assert
            AuthenticationManager.Verify(x => x.SignOut("TempState"), Times.Once);
        }

        [Test]
        public async Task ThenTheAuthenticatedUserIsSignedIn()
        {
            //Act
            await _authenticateCoreStrategy.Authenticate(OwinContext.Object);

            //Assert
            AuthenticationManager.Verify(x => x.SignIn(It.Is<ClaimsIdentity>(c => c.AuthenticationType == "Cookies")), Times.Once);
        }

        [Test]
        public async Task ThenTheClaimsAreCorrectlyPopulated()
        {
            //act
            var actual = await _authenticateCoreStrategy.Authenticate(OwinContext.Object);

            //Assert
            Assert.IsNotEmpty(actual.Identity.Claims);
            Assert.AreEqual(AccessToken, actual.Identity.Claims.First(c => c.Type == "access_token").Value);
            Assert.AreEqual(RefreshToken, actual.Identity.Claims.First(c => c.Type == "refresh_token").Value);
            Assert.AreEqual(LoggedInName, actual.Identity.Claims.First(c => c.Type == "name").Value);
            Assert.IsTrue(actual.Identity.Claims.Count(c => c.Type == "refresh_token") == 1);
        }

        [Test]
        public async Task ThenTheClaimsAreAddedFromTheUserInfoClient()
        {
            //act
            await _authenticateCoreStrategy.Authenticate(OwinContext.Object);

            //Assert
            _buildUserInfoClientUrl.Verify(x=>x.GetUserClaims(_oidcMiddlewareOptions,AccessToken));
        }

        [Test]
        public async Task ThenTheTokenIsCorrectlyValidated()
        {
            //act
            await _authenticateCoreStrategy.Authenticate(OwinContext.Object);

            //Assert
            _securityTokenValidation.Verify(x=>x.ValidateToken(IdentityToken, Nonce));
        }
    }
}
