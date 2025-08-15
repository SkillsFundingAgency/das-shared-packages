using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Moq;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.GovUK.Auth.Validation;

namespace SFA.DAS.GovUK.Auth.UnitTests.AppStart
{
    [TestFixture]
    public class GovUkOpenIdConnectEventsTests
    {
        private Mock<IGovUkAuthenticationService> _authServiceMock;
        private Mock<ICoreIdentityJwtValidator> _jwtValidatorMock;
        private GovUkOidcConfiguration _config;
        private Mock<IOptions<GovUkOidcConfiguration>> _configMock;
        private GovUkOpenIdConnectEvents _sut;
        private const string RedirectUrl = "/signed-out";
        private const string SuspendedUrl = "/suspended";

        [SetUp]
        public void SetUp()
        {
            _config = new GovUkOidcConfiguration
            {
                RequestedUserInfoClaims = "CoreIdentityJWT,Address"
            };
            _configMock = new Mock<IOptions<GovUkOidcConfiguration>>();
            _configMock.Setup(x => x.Value).Returns(_config);

            _authServiceMock = new Mock<IGovUkAuthenticationService>();
            _jwtValidatorMock = new Mock<ICoreIdentityJwtValidator>();

            _sut = new GovUkOpenIdConnectEvents(_configMock.Object, _authServiceMock.Object, _jwtValidatorMock.Object, RedirectUrl, SuspendedUrl);
        }

        [Test]
        public async Task RemoteFailure_CorrelationFailed_RedirectsAndHandles()
        {
            var context = new RemoteFailureContext(
                new DefaultHttpContext(),
                new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions(),
                new Exception("Correlation failed"));
            
            await _sut.RemoteFailure(context);

            context.Response.StatusCode.Should().Be(302);
            context.Response.Headers["Location"].ToString().Should().Be("/");
            context.Result.Handled.Should().BeTrue();
        }

        [Test]
        public async Task RedirectToIdentityProvider_AddsVtrAndClaims_WhenEnableVerify()
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                ["enableVerify"] = "true"
            });

            var protocolMessage = new OpenIdConnectMessage();

            var mockStateFormat = new Mock<ISecureDataFormat<AuthenticationProperties>>();
            mockStateFormat
                .Setup(m => m.Protect(It.IsAny<AuthenticationProperties>()))
                .Returns("mock-protected-state");

            var context = new RedirectContext(
                new DefaultHttpContext(),
                new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions { StateDataFormat = mockStateFormat.Object },
                properties)
            {
                ProtocolMessage = protocolMessage
            };

            await _sut.RedirectToIdentityProvider(context);

            protocolMessage.Parameters["vtr"].Should().NotBeNull();
            protocolMessage.Parameters["claims"].Should().Contain("userinfo");
        }

        [Test]
        public async Task TokenResponseReceived_StoresIdTokenAndLoadsDid_WhenVerifyEnabled()
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                ["enableVerify"] = "true"
            });

            var context = new TokenResponseReceivedContext(new DefaultHttpContext(),
                new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions(),
                new ClaimsPrincipal(),
                properties)
            {
                Properties = properties,
                TokenEndpointResponse = new OpenIdConnectMessage
                {
                    IdToken = "id-token"
                }
            };

            await _sut.TokenResponseReceived(context);

            _jwtValidatorMock.Verify(x => x.LoadDidDocument(), Times.Once);
            context.Properties!.GetTokenValue("id_token").Should().Be("id-token");
        }

        [Test]
        public async Task AuthorizationCodeReceived_HandlesRedemption_WhenTokensPresent()
        {
            _authServiceMock.Setup(x => x.GetToken(It.IsAny<OpenIdConnectMessage>()))
                .ReturnsAsync(new Token
                {
                    AccessToken = "access-token",
                    IdToken = "id-token"
                });

            var context = new AuthorizationCodeReceivedContext(new DefaultHttpContext(),
                new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions(),
                new AuthenticationProperties())
            {
                TokenEndpointRequest = new OpenIdConnectMessage(),
                Properties = new AuthenticationProperties()
            };

            await _sut.AuthorizationCodeReceived(context);

            context.Properties.GetTokenValue("access_token").Should().Be("access-token");
            context.Properties.GetTokenValue("id_token").Should().Be("id-token");
            context.HandledCodeRedemption.Should().BeTrue();
        }

        [Test]
        public async Task SignedOutCallbackRedirect_DeletesCookieAndRedirects()
        {
            // Arrange
            var cookiesMock = new Mock<IResponseCookies>();
            var responseMock = new Mock<HttpResponse>();
            var contextMock = new Mock<HttpContext>();

            var headers = new HeaderDictionary();
            responseMock.Setup(r => r.Cookies).Returns(cookiesMock.Object);
            responseMock.Setup(r => r.Redirect(It.IsAny<string>()))
                        .Callback<string>(url => headers["Location"] = url);

            contextMock.Setup(c => c.Response).Returns(responseMock.Object);

            var context = new RemoteSignOutContext(
                contextMock.Object,
                new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions(),
                new OpenIdConnectMessage());

            // Act
            await _sut.SignedOutCallbackRedirect(context);

            // Assert
            headers["Location"].ToString().Should().Be(RedirectUrl);
            cookiesMock.Verify(c => c.Delete(GovUkConstants.AuthCookieName), Times.Once);
        }


        [Test]
        public async Task TokenValidated_CallsPopulateAccountClaims()
        {
            var context = new TokenValidatedContext(new DefaultHttpContext(),
                new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions(),
                new ClaimsPrincipal(),
                new AuthenticationProperties());

            await _sut.TokenValidated(context);

            _authServiceMock.Verify(x => x.PopulateAccountClaims(context), Times.Once);
        }
    }
}
