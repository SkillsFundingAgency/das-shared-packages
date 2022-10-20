using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Owin.Security;
using Microsoft.Owin.Testing;
using Moq;
using NUnit.Framework;
using SFA.DAS.OidcMiddleware.Caches;
using SFA.DAS.OidcMiddleware.Clients;
using SFA.DAS.OidcMiddleware.Validators;

namespace SFA.DAS.OidcMiddleware.UnitTests
{
    public static class CodeFlowAuthenticationHanderTests
    {
        public class WhenChallengingAndTheResponseCodeIs401 : Test
        {
            private const string AuthenticationType = "code";
            private const string AuthorizeEndpoint = "https://authorize.endpoint";
            private const string ClientId = "eas";

            private TestServer _server;
            private OidcMiddlewareOptions _options;
            private readonly Mock<INonceCache> _nonceCache = new Mock<INonceCache>();
            private HttpResponseMessage _response;

            protected override void Given()
            {
                _options = new OidcMiddlewareOptions
                {
                    AuthorizeEndpoint = AuthorizeEndpoint,
                    ClientId = ClientId,
                    NonceCache = _nonceCache.Object
                };

                _nonceCache.Setup(c => c.SetNonceAsync(It.IsAny<IAuthenticationManager>(), It.IsAny<string>()));

                _server = CreateServer(
                    app => app.UseCodeFlowAuthentication(_options),
                    context =>
                    {
                        context.Authentication.Challenge(AuthenticationType);
                        return true;
                    });
            }

            protected override async Task When()
            {
                _response = await _server.CreateRequest("/").GetAsync();
            }

            [Test]
            public void ThenShouldStoreNonce()
            {
                _nonceCache.Verify(c => c.SetNonceAsync(It.IsAny<IAuthenticationManager>(), It.IsAny<string>()), Times.Once);
            }

            [Test]
            public void ThenShouldRedirectToAuthority()
            {
                Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));

                var absoluteUri = _response.Headers.Location.AbsoluteUri;

                Assert.That(absoluteUri, Does.Contain(AuthorizeEndpoint));
                Assert.That(absoluteUri, Does.Contain("response_type=" + AuthenticationType));
                Assert.That(absoluteUri, Does.Contain("client_id=" + ClientId));
                Assert.That(absoluteUri, Does.Contain("redirect_uri=" + WebUtility.UrlEncode("http://localhost")));
                Assert.That(absoluteUri, Does.Contain("state="));
            }
        }

        public class WhenChallengingAndResponseCodeIsNot401 : Test
        {
            private TestServer _server;
            private HttpResponseMessage _response;

            protected override void Given()
            {
                _server = CreateServer(
                    app => app.UseCodeFlowAuthentication(new OidcMiddlewareOptions()),
                    context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        return true;
                    });
            }

            protected override async Task When()
            {
                _response = await _server.CreateRequest("/").GetAsync();
            }

            [Test]
            public void ThenShouldNotRedirectToAuthority()
            {
                Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            }
        }

        public class WhenAuthenticatingAndQuerystringCodeAndStateParametersAreAvailable : Test
        {
            private const string AccessToken = "myaccesstoken";
            private const string ClientId = "username";
            private const string ClientSecret = "password";
            private const string Code = "abc";
            private const string IdentityToken = "12345";
            private const string LoggedInName = "johndoe";
            private const string Nonce = "nonce";
            private const string RefreshToken = "myrefreshtoken";
            private const string TokenEndpoint = "https://authorize.endpoint";
            private const string OriginalUrl = "http://localhost/foo/bar?foo=foo&bar=bar";

            private TestServer _server;
            private OidcMiddlewareOptions _options;
            private readonly Mock<INonceCache> _nonceCache = new Mock<INonceCache>();
            private readonly Mock<ITokenClient> _tokenClient = new Mock<ITokenClient>();
            private readonly Mock<ITokenValidator> _tokenValidator = new Mock<ITokenValidator>();
            private readonly Mock<IUserInfoClient> _userInfoClient = new Mock<IUserInfoClient>();
            private string _state;
            private string _url;
            private HttpResponseMessage _response;
            private ClaimsIdentity _identity;

            protected override void Given()
            {
                _options = new OidcMiddlewareOptions
                {
                    AuthenticatedCallback = i => _identity = i,
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    NonceCache = _nonceCache.Object,
                    TokenClient = _tokenClient.Object,
                    TokenEndpoint = TokenEndpoint,
                    TokenValidator = _tokenValidator.Object,
                    UserInfoClient = _userInfoClient.Object
                };

                _nonceCache.Setup(c => c.GetNonceAsync(It.IsAny<IAuthenticationManager>()))
                    .ReturnsAsync(Nonce);

                var msg = new HttpResponseMessage(HttpStatusCode.Accepted);
                var content = new StringContent("{\"id_token\": \"" + IdentityToken + "\",\"access_token\": \"" +
                                              AccessToken + "\",\"refresh_token\": \"" + RefreshToken + "\"}");
                msg.Content = content;

                var tokenResponse = ProtocolResponse.FromHttpResponseAsync<TokenResponse>(msg); 
                
                _tokenClient
                    .Setup(c => c.RequestAuthorizationCodeAsync(It.IsAny<HttpMessageInvoker>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Uri>()))
                    .Returns(tokenResponse);

                _tokenValidator.Setup(v => v.ValidateToken(It.IsAny<OidcMiddlewareOptions>(), It.IsAny<string>(), It.IsAny<string>()));

                _userInfoClient.Setup(c => c.GetUserClaims(It.IsAny<HttpClient>(), It.IsAny<OidcMiddlewareOptions>(), It.IsAny<string>()))
                    .ReturnsAsync(new List<Claim> { new Claim("name", LoggedInName) });

                _server = CreateServer(app => app.UseCodeFlowAuthentication(_options));

                _state = _options.StateDataFormat.Protect(new AuthenticationProperties { RedirectUri = OriginalUrl });
                _url = $"{OriginalUrl}&code={Code}&state={_state}";
            }

            protected override async Task When()
            {
                _response = await _server.CreateRequest(_url).GetAsync();
            }

            [Test]
            public void ThenShouldRequestToken()
            {
                _tokenClient.Verify(c => c.RequestAuthorizationCodeAsync(It.IsAny<HttpMessageInvoker>(), TokenEndpoint, ClientId, ClientSecret, Code, new Uri(_url)), Times.Once);
            }

            [Test]
            public void ThenShouldValidateToken()
            {
                _tokenValidator.Verify(v => v.ValidateToken(_options, IdentityToken, Nonce));
            }

            [Test]
            public void ThenShouldAddClaims()
            {
                Assert.That(_identity.Claims, Is.Not.Empty);
                Assert.That(_identity.Claims.Single(c => c.Type == "access_token").Value, Is.EqualTo(AccessToken));
                Assert.That(_identity.Claims.Single(c => c.Type == "refresh_token").Value, Is.EqualTo(RefreshToken));
            }

            [Test]
            public void ThenShouldAddUserClaims()
            {
                _userInfoClient.Verify(c => c.GetUserClaims(It.IsAny<HttpClient>(), _options, AccessToken), Times.Once);

                Assert.That(_identity.Claims.Single(c => c.Type == "name").Value, Is.EqualTo(LoggedInName));
            }

            [Test]
            public void ThenShouldCreateAuthenticationCookie()
            {
                Assert.That(_response.Headers.GetValues("Set-Cookie").Single(), Does.Contain(".AspNet.Cookies"));
            }

            [Test]
            public void ThenShouldRedirectToOriginalUrl()
            {
                Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
                Assert.That(_response.Headers.Location.AbsoluteUri, Is.EqualTo(OriginalUrl));
            }
        }

        public class WhenAuthenticatingAndQuerystringCodeAndStateParametersAreNotAvailable : Test
        {
            private TestServer _server;
            private HttpResponseMessage _response;

            protected override void Given()
            {
                _server = CreateServer(app => app.UseCodeFlowAuthentication(new OidcMiddlewareOptions()));
            }

            protected override async Task When()
            {
                _response = await _server.CreateRequest("http://localhost/foo/bar?foo=foo&bar=bar").GetAsync();
            }

            [Test]
            public void ThenShouldNotCreateAuthenticationCookie()
            {
                Assert.That(_response.Headers.Any(h => h.Key == "Set-Cookie"), Is.False);
            }
        }
    }
}