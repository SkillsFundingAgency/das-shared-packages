using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using SFA.DAS.ApiTokens.Lib;

namespace SFA.DAS.ApiTokens.Tests
{
    [TestFixture]
    public class JwtTokenServiceTests
    {
        private readonly IEnumerable<string> _validAudiences = new[] {"http://my.website.com", "http://other.website.com"};
        private readonly IEnumerable<string> _validIssuers = new[] {"http://my.tokenissuer.com", "http://other.tokenissuer.com"};

        [Test]
        public void ShouldCreateAToken()
        {
            // arrange
            var value = "some value";
            var encoder = new JwtTokenService("this is a secret");

            // act
            var token = encoder.Encode(value, "http://my.website.com", "http://my.tokenissuer.com");

            // assert
            Assert.That(token, Is.Not.Empty);
        }

        [Test]
        public void ShouldDecodeAToken()
        {
            // arrange
            var value = "some value";
            var service = new JwtTokenService("this is a secret");
            var token = service.Encode(value, "http://my.website.com", "http://my.tokenissuer.com");

            // act
            var decoded = service.Decode(token, _validAudiences, _validIssuers);

            // assert
            Assert.That(decoded, Is.EqualTo(value));
        }

        [Test]
        public void ShouldFailToDecodeIfSecretIsInvalid()
        {
            // arrange
            var value = "some value";
            var encoder = new JwtTokenService("this is a secret");
            var token = encoder.Encode(value, "http://my.website.com", "http://my.tokenissuer.com");

            // create a separate service for decoding
            var decoder = new JwtTokenService("this is a different secret"); // different secret to encoder

            // act
            ActualValueDelegate<string> testDelegate = () => decoder.Decode(token, _validAudiences, _validIssuers);

            // assert
            Assert.That(testDelegate, Throws.TypeOf<SignatureVerificationFailedException>());
        }

        [Test]
        public void ShouldFailToDecodeIfIssuerIsInvalid()
        {
            // arrange
            var value = "some value";
            var service = new JwtTokenService("this is a secret");
            var token = service.Encode(value, "http://my.website.com", "http://unknown.tokenissuer.com"); // unknown issuer

            // act
            ActualValueDelegate<string> testDelegate = () => service.Decode(token, _validAudiences, _validIssuers);

            // assert
            Assert.That(testDelegate, Throws.TypeOf<SecurityTokenInvalidIssuerException>());
        }

        [Test]
        public void ShouldFailToDecodeIfAudienceIsInvalid()
        {
            // arrange
            var value = "some value";
            var service = new JwtTokenService("this is a secret");
            var token = service.Encode(value, "http://unknown.website.com", "http://my.tokenissuer.com"); // unknown audience

            // act
            ActualValueDelegate<string> testDelegate = () => service.Decode(token, _validAudiences, _validIssuers);

            // assert
            Assert.That(testDelegate, Throws.TypeOf<SecurityTokenInvalidAudienceException>());
        }

        [Test]
        public void ShouldFailToDecodeIfTokenIsExpired()
        {
            // arrange
            var value = "some value";
            var service = new JwtTokenService("this is a secret", 0.0d); // no clock skew allowed
            var token = service.Encode(value, "http://my.website.com", "http://my.tokenissuer.com", 0.1d); // short lifetime

            Thread.Sleep(250);

            // act
            ActualValueDelegate<string> testDelegate = () => service.Decode(token, _validAudiences, _validIssuers);

            // assert
            Assert.That(testDelegate, Throws.TypeOf<SecurityTokenExpiredException>());
        }
    }
}
