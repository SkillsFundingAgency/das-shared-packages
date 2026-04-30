using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
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
        private Mock<ISigningCredentialsProvider> _signingCredentialsProvider;
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
                ClientId = "test-client-id",
                BaseUrl = "https://oidc.example.gov.uk",
                RequestedUserInfoClaims = "CoreIdentityJWT,Address"
            };
            _configMock = new Mock<IOptions<GovUkOidcConfiguration>>();
            _configMock.Setup(x => x.Value).Returns(_config);

            _authServiceMock = new Mock<IGovUkAuthenticationService>();
            _jwtValidatorMock = new Mock<ICoreIdentityJwtValidator>();
            _signingCredentialsProvider = new Mock<ISigningCredentialsProvider>();

            var rsa = RSA.Create(2048);
            var testSigningCredentials = new SigningCredentials(
                new RsaSecurityKey(rsa),
                SecurityAlgorithms.RsaSha256);
            _signingCredentialsProvider
                .Setup(x => x.GetSigningCredentials())
                .Returns(testSigningCredentials);

            _sut = new GovUkOpenIdConnectEvents(
                _configMock.Object,
                _authServiceMock.Object,
                _jwtValidatorMock.Object,
                _signingCredentialsProvider.Object,
                RedirectUrl,
                SuspendedUrl);
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

            var context = BuildRedirectContext(properties);

            await _sut.RedirectToIdentityProvider(context);

            context.ProtocolMessage.Parameters["vtr"].Should().NotBeNull();
            context.ProtocolMessage.Parameters["claims"].Should().Contain("userinfo");
        }

        [Test]
        public async Task RedirectToIdentityProvider_SetsJarRequestParameter()
        {
            var context = BuildRedirectContext();

            await _sut.RedirectToIdentityProvider(context);

            context.ProtocolMessage.Parameters.Should().ContainKey("request");
            context.ProtocolMessage.Parameters["request"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task RedirectToIdentityProvider_JarContainsCoreAuthorizationClaims()
        {
            var message = new OpenIdConnectMessage
            {
                ResponseType = "code",
                ClientId = "test-client-id",
                RedirectUri = "https://localhost/sign-in",
                Scope = "openid email",
                Nonce = "test-nonce"
            };
            message.Parameters.Add("vtr", "[\"Cl.Cm\"]");

            var context = BuildRedirectContext(protocolMessage: message);

            await _sut.RedirectToIdentityProvider(context);

            var jwt = ReadJar(context);
            jwt.Payload["response_type"].Should().Be("code");
            jwt.Payload["client_id"].Should().Be("test-client-id");
            jwt.Payload["redirect_uri"].Should().Be("https://localhost/sign-in");
            jwt.Payload["nonce"].Should().Be("test-nonce");
            jwt.Payload["ui_locales"].Should().Be("en");
        }

        [Test]
        public async Task RedirectToIdentityProvider_JarStateMatchesProtectedProperties()
        {
            var context = BuildRedirectContext(stateFormatResult: "expected-protected-state");

            await _sut.RedirectToIdentityProvider(context);

            var jwt = ReadJar(context);
            jwt.Payload["state"].Should().Be("expected-protected-state");
        }

        [Test]
        public async Task RedirectToIdentityProvider_JarStateIsProtectedWithRedirectUriKeyInProperties()
        {
            const string callbackUri = "https://localhost/sign-in";
            AuthenticationProperties? capturedProperties = null;

            var mockStateFormat = new Mock<ISecureDataFormat<AuthenticationProperties>>();
            mockStateFormat
                .Setup(m => m.Protect(It.IsAny<AuthenticationProperties>()))
                .Callback<AuthenticationProperties>(p => capturedProperties = p)
                .Returns("mock-state");

            var message = new OpenIdConnectMessage { RedirectUri = callbackUri };
            message.Parameters.Add("vtr", "[\"Cl.Cm\"]");

            var context = BuildRedirectContext(protocolMessage: message, stateDataFormat: mockStateFormat.Object);

            await _sut.RedirectToIdentityProvider(context);

            capturedProperties.Should().NotBeNull();
            capturedProperties!.Items.Should().ContainKey(OpenIdConnectDefaults.RedirectUriForCodePropertiesKey);
            capturedProperties.Items[OpenIdConnectDefaults.RedirectUriForCodePropertiesKey].Should().Be(callbackUri);
        }

        [Test]
        public async Task RedirectToIdentityProvider_DoesNotSetProtocolMessageState_SoThatJARAndQueryStringStateAreIdentical()
        {
            var context = BuildRedirectContext();

            await _sut.RedirectToIdentityProvider(context);

            context.ProtocolMessage.State.Should().BeNull();
        }

        [Test]
        public async Task RedirectToIdentityProvider_JarIncludesPkceClaimsWhenPresent()
        {
            var message = new OpenIdConnectMessage { RedirectUri = "https://localhost/sign-in" };
            message.Parameters.Add("vtr", "[\"Cl.Cm\"]");
            message.Parameters.Add("code_challenge", "abc123challenge");
            message.Parameters.Add("code_challenge_method", "S256");

            var context = BuildRedirectContext(protocolMessage: message);

            await _sut.RedirectToIdentityProvider(context);

            var jwt = ReadJar(context);
            jwt.Payload["code_challenge"].Should().Be("abc123challenge");
            jwt.Payload["code_challenge_method"].Should().Be("S256");
        }

        [Test]
        public async Task RedirectToIdentityProvider_JarExcludesPkceClaimsWhenAbsent()
        {
            var context = BuildRedirectContext();

            await _sut.RedirectToIdentityProvider(context);

            var jwt = ReadJar(context);
            jwt.Payload.ContainsKey("code_challenge").Should().BeFalse();
            jwt.Payload.ContainsKey("code_challenge_method").Should().BeFalse();
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

            await _sut.SignedOutCallbackRedirect(context);

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


        private RedirectContext BuildRedirectContext(
            AuthenticationProperties? properties = null,
            OpenIdConnectMessage? protocolMessage = null,
            ISecureDataFormat<AuthenticationProperties>? stateDataFormat = null,
            string stateFormatResult = "mock-protected-state")
        {
            properties ??= new AuthenticationProperties();

            if (protocolMessage == null)
            {
                protocolMessage = new OpenIdConnectMessage
                {
                    RedirectUri = "https://localhost/sign-in"
                };
                protocolMessage.Parameters.Add("vtr", "[\"Cl.Cm\"]");
            }

            if (stateDataFormat == null)
            {
                var mockStateFormat = new Mock<ISecureDataFormat<AuthenticationProperties>>();
                mockStateFormat
                    .Setup(m => m.Protect(It.IsAny<AuthenticationProperties>()))
                    .Returns(stateFormatResult);
                stateDataFormat = mockStateFormat.Object;
            }

            return new RedirectContext(
                new DefaultHttpContext(),
                new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions { StateDataFormat = stateDataFormat },
                properties)
            {
                ProtocolMessage = protocolMessage
            };
        }

        private static JwtSecurityToken ReadJar(RedirectContext context)
        {
            var jarToken = context.ProtocolMessage.Parameters["request"];
            return new JwtSecurityTokenHandler().ReadJwtToken(jarToken);
        }
    }
}
