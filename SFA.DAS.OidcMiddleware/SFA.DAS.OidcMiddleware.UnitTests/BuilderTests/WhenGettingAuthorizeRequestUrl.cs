using System;
using NUnit.Framework;
using SFA.DAS.OidcMiddleware.Builders;

namespace SFA.DAS.OidcMiddleware.UnitTests.BuilderTests
{
    public class WhenGettingAuthorizeRequestUrl
    {
        [Test]
        public void ThenItShouldReturnCorrectUrl()
        {
            var authorityUrl = "http://localhost";
            var requestUrl = new Uri("http://unit.tests:1234/foo/bar?foo=foo&bar=bar");
            var clientId = "TheClientId";
            var scopes = "RequestedScopes";
            var state = "abc";
            var nonce = "123";
            var buildAuthorizeRequestUrl = new BuildAuthorizeRequestUrl();
            var url = buildAuthorizeRequestUrl.GetAuthorizeRequestUrl(authorityUrl, requestUrl, clientId, scopes, state, nonce);

            Assert.AreEqual(
                "http://localhost/?client_id=TheClientId&response_type=code&scope=RequestedScopes&redirect_uri=" + 
                "http%3A%2F%2Funit.tests%3A1234%2Ffoo%2Fbar%3Ffoo%3Dfoo%26bar%3Dbar&state=abc&nonce=123", url);
        }
    }
}