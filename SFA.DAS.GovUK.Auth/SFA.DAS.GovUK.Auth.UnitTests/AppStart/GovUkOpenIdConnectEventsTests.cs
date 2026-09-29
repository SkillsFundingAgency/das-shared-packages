using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
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
            // Arrange
            var context = new RemoteFailureContext(
                new DefaultHttpContext(),
                new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions(),
                new Exception("Correlation failed"));

            // Act
            await _sut.RemoteFailure(context);

            // Assert
            context.Response.StatusCode.Should().Be(302);
            context.Response.Headers["Location"].ToString().Should().Be("/");
            context.Result.Handled.Should().BeTrue();
        }

        [Test]
        public async Task RedirectToIdentityProvider_AddsVtrAndClaims_WhenVerifyEnabled()
        {
            // Arrange
            var properties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                ["enableVerify"] = "true"
            });

            var context = BuildRedirectContext(properties);

            // Act
            await _sut.RedirectToIdentityProvider(context);

            // Assert
            context.ProtocolMessage.Parameters["vtr"].Should().NotBeNull();
            context.ProtocolMessage.Parameters["claims"].Should().Contain("userinfo");
        }

        [Test]
        public async Task RedirectToIdentityProvider_JarContainsVerifyVtrAndClaimsAsJsonObject_WhenVerifyEnabled()
        {
            // Arrange
            var properties = new AuthenticationProperties(new Dictionary<string, string?>
            {
                ["enableVerify"] = "true"
            });

            var message = new OpenIdConnectMessage
            {
                ResponseType = "code",
                ClientId = "test-client-id",
                RedirectUri = "https://localhost/sign-in",
                Scope = "openid email phone",
                Nonce = "test-nonce"
            };

            var context = BuildRedirectContext(properties, message);

            // Act
            await _sut.RedirectToIdentityProvider(context);

            // Assert
            using var payload = ReadJarPayload(context);

            var root = payload.RootElement;

            root.GetProperty("vtr")[0].GetString().Should().Be("Cl.Cm.P2");

            var claims = root.GetProperty("claims");
            claims.ValueKind.Should().Be(JsonValueKind.Object);

            var userInfo = claims.GetProperty("userinfo");
            userInfo.ValueKind.Should().Be(JsonValueKind.Object);

            userInfo.TryGetProperty("https://vocab.account.gov.uk/v1/coreIdentityJWT", out _)
                .Should().BeTrue();

            userInfo.TryGetProperty("https://vocab.account.gov.uk/v1/address", out _)
                .Should().BeTrue();
        }

        [Test]
        public async Task RedirectToIdentityProvider_JarContainsStandardVtrAndNoClaims_WhenVerifyNotEnabled()
        {
            // Arrange
            var message = new OpenIdConnectMessage
            {
                ResponseType = "code",
                ClientId = "test-client-id",
                RedirectUri = "https://localhost/sign-in",
                Scope = "openid email phone",
                Nonce = "test-nonce"
            };

            var context = BuildRedirectContext(protocolMessage: message);

            // Act
            await _sut.RedirectToIdentityProvider(context);

            // Assert
            using var payload = ReadJarPayload(context);

            var root = payload.RootElement;

            root.GetProperty("vtr")[0].GetString().Should().Be("Cl.Cm");
            root.TryGetProperty("claims", out _).Should().BeFalse();
        }

        [Test]
        public async Task RedirectToIdentityProvider_SetsJarRequestParameter()
        {
            // Arrange
            var context = BuildRedirectContext();

            // Act
            await _sut.RedirectToIdentityProvider(context);

            // Assert
            context.ProtocolMessage.Parameters.Should().ContainKey("request");
            context.ProtocolMessage.Parameters["request"].Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task RedirectToIdentityProvider_JarContainsCoreAuthorizationClaims()
        {
            // Arrange
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

            // Act
            await _sut.RedirectToIdentityProvider(context);

            // Assert
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
            // Arrange
            var context = BuildRedirectContext(stateFormatResult: "expected-protected-state");

            // Act
            await _sut.RedirectToIdentityProvider(context);

            // Assert
            var jwt = ReadJar(context);
            jwt.Payload["state"].Should().Be("expected-protected-state");
        }

        [Test]
        public async Task RedirectToIdentityProvider_JarStateIsProtectedWithRedirectUriKeyInProperties()
        {
            // Arrange
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

            // Act
            await _sut.RedirectToIdentityProvider(context);

            // Assert
            capturedProperties.Should().NotBeNull();
            capturedProperties!.Items.Should().ContainKey(OpenIdConnectDefaults.RedirectUriForCodePropertiesKey);
            capturedProperties.Items[OpenIdConnectDefaults.RedirectUriForCodePropertiesKey].Should().Be(callbackUri);
        }

        [Test]
        public async Task RedirectToIdentityProvider_DoesNotSetProtocolMessageState_SoThatJARAndQueryStringStateAreIdentical()
        {
            // Arrange
            var context = BuildRedirectContext();

            // Act
            await _sut.RedirectToIdentityProvider(context);

            // Assert
            context.ProtocolMessage.State.Should().BeNull();
        }

        [Test]
        public async Task RedirectToIdentityProvider_JarIncludesPkceClaimsWhenPresent()
        {
            // Arrange
            var message = new OpenIdConnectMessage { RedirectUri = "https://localhost/sign-in" };
            message.Parameters.Add("vtr", "[\"Cl.Cm\"]");
            message.Parameters.Add("code_challenge", "abc123challenge");
            message.Parameters.Add("code_challenge_method", "S256");

            var context = BuildRedirectContext(protocolMessage: message);

            // Act
            await _sut.RedirectToIdentityProvider(context);

            // Assert
            var jwt = ReadJar(context);
            jwt.Payload["code_challenge"].Should().Be("abc123challenge");
            jwt.Payload["code_challenge_method"].Should().Be("S256");
        }

        [Test]
        public async Task RedirectToIdentityProvider_JarExcludesPkceClaimsWhenAbsent()
        {
            // Arrange
            var context = BuildRedirectContext();

            // Act
            await _sut.RedirectToIdentityProvider(context);

            // Assert
            var jwt = ReadJar(context);
            jwt.Payload.ContainsKey("code_challenge").Should().BeFalse();
            jwt.Payload.ContainsKey("code_challenge_method").Should().BeFalse();
        }

        [Test]
        public async Task TokenResponseReceived_StoresIdTokenAndLoadsDid_WhenVerifyEnabled()
        {
            // Arrange
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

            // Act
            await _sut.TokenResponseReceived(context);

            // Assert
            _jwtValidatorMock.Verify(x => x.LoadDidDocument(), Times.Once);
            context.Properties!.GetTokenValue("id_token").Should().Be("id-token");
        }

        [Test]
        public async Task AuthorizationCodeReceived_HandlesRedemption_WhenTokensPresent()
        {
            // Arrange
            _authServiceMock.Setup(x => x.GetToken(It.IsAny<OpenIdConnectMessage>()))
                .ReturnsAsync(new Token
                {
                    AccessToken = "access-token",
                    IdToken = "id-token"
                });

            var context = new AuthorizationCodeReceivedContext(
                new DefaultHttpContext(),
                new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions(),
                new AuthenticationProperties())
            {
                TokenEndpointRequest = new OpenIdConnectMessage(),
                Properties = new AuthenticationProperties()
            };

            // Act
            await _sut.AuthorizationCodeReceived(context);

            // Assert
            context.HandledCodeRedemption.Should().BeTrue();
            context.TokenEndpointResponse!.AccessToken.Should().Be("access-token");
            context.TokenEndpointResponse.IdToken.Should().Be("id-token");
            context.Properties.GetTokenValue("id_token").Should().Be("id-token");
            context.Properties.GetTokenValue("access_token").Should().BeNull();
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
            context.Result.Handled.Should().BeTrue();
        }

        [Test]
        public async Task TokenValidated_CallsPopulateAccountClaims()
        {
            // Arrange
            var context = new TokenValidatedContext(new DefaultHttpContext(),
                new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions(),
                new ClaimsPrincipal(),
                new AuthenticationProperties());

            // Act
            await _sut.TokenValidated(context);

            // Assert
            _authServiceMock.Verify(x => x.PopulateAccountClaims(context), Times.Once);
            context.Properties!.Items["suspended_redirect"].Should().Be(SuspendedUrl);
        }

        [Test]
        public async Task UserInformationReceived_ReplacesUserInfoClaim_WithCompleteUser()
        {
            // Arrange
            var user = new GovUkUser
            {
                Sub = "user-123",
                Email = "jane@example.com",
                EmailVerified = true,
                Addresses = new List<GovUkAddress>
                {
                    new GovUkAddress { StreetName = "Test Lane" }
                },
                CoreIdentityJwt = new GovUkCoreIdentityJwt
                {
                    Sub = "user-123",
                    Vot = "Cl.Cm.P2",
                    Vc = new GovUkCoreIdentityCredential
                    {
                        CredentialSubject = new GovUkCredentialSubject
                        {
                            Names = new List<GovUkName>
                    {
                        new GovUkName
                        {
                            NameParts = new List<GovUkNamePart>
                            {
                                new GovUkNamePart
                                {
                                    Type = "GivenName",
                                    Value = "Jane"
                                }
                            }
                        }
                    }
                        }
                    }
                }
            };

            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(GovUkUserClaimTypes.UserInfo, "{}"));

            using var userDocument = JsonDocument.Parse(JsonSerializer.Serialize(user));

            var context = new UserInformationReceivedContext(
                new DefaultHttpContext(),
                new AuthenticationScheme("oidc", null, typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions(),
                new ClaimsPrincipal(identity),
                new AuthenticationProperties())
            {
                User = userDocument
            };

            // Act
            await _sut.UserInformationReceived(context);

            // Assert
            identity.FindAll(GovUkUserClaimTypes.UserInfo).Should().ContainSingle();

            var storedUser = JsonSerializer.Deserialize<GovUkUser>(
                identity.FindFirst(GovUkUserClaimTypes.UserInfo)!.Value);

            storedUser.Should().BeEquivalentTo(user);
        }

        private static RedirectContext BuildRedirectContext(
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

        private static JsonDocument ReadJarPayload(RedirectContext context)
        {
            var jwt = ReadJar(context);
            var payloadJson = Base64UrlEncoder.Decode(jwt.EncodedPayload);

            return JsonDocument.Parse(payloadJson);
        }
    }
}
