using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using SFA.DAS.ApiTokens.Lib;

namespace SFA.DAS.ApiTokens.Tests;

[TestFixture]
public class JwtTokenServiceTests
{
    // HS256 requires minimum 256 bits (32 bytes) for the secret key
    // Using 64 characters to ensure we exceed the minimum requirement
    private const string ValidSecret = "this-is-a-very-long-secret-key-for-jwt-signing-12345678901234";  // 64 chars
    private const string DifferentSecret = "another-very-long-secret-key-for-jwt-signing-1234567890123";  // 64 chars different
    
    private readonly IEnumerable<string> _validAudiences = new[] { "http://my.website.com", "http://other.website.com" };
    private readonly IEnumerable<string> _validIssuers = new[] { "http://my.tokenissuer.com", "http://other.tokenissuer.com" };

    [Test]
    public void ShouldCreateAToken()
    {
        // arrange
        var value = "some value";
        var encoder = new JwtTokenService(ValidSecret);

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
        var service = new JwtTokenService(ValidSecret);
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
        var encoder = new JwtTokenService(ValidSecret);
        var token = encoder.Encode(value, "http://my.website.com", "http://my.tokenissuer.com");

        // create a separate service for decoding
        var decoder = new JwtTokenService(DifferentSecret); // different secret to encoder

        // act & assert - JWT v8.x throws SecurityTokenSignatureKeyNotFoundException when key doesn't match
        Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() => 
            decoder.Decode(token, _validAudiences, _validIssuers));
    }

    [Test]
    public void ShouldFailToDecodeIfIssuerIsInvalid()
    {
        // arrange
        var value = "some value";
        var service = new JwtTokenService(ValidSecret);
        var token = service.Encode(value, "http://my.website.com", "http://unknown.tokenissuer.com"); // unknown issuer

        // act & assert
        Assert.Throws<SecurityTokenInvalidIssuerException>(() => 
            service.Decode(token, _validAudiences, _validIssuers));
    }

    [Test]
    public void ShouldFailToDecodeIfAudienceIsInvalid()
    {
        // arrange
        var value = "some value";
        var service = new JwtTokenService(ValidSecret);
        var token = service.Encode(value, "http://unknown.website.com", "http://my.tokenissuer.com"); // unknown audience

        // act & assert
        Assert.Throws<SecurityTokenInvalidAudienceException>(() => 
            service.Decode(token, _validAudiences, _validIssuers));
    }

    [Test]
    public void ShouldFailToDecodeIfTokenIsExpired()
    {
        // arrange
        var value = "some value";
        var service = new JwtTokenService(ValidSecret, 0.0d); // no clock skew allowed
        var token = service.Encode(value, "http://my.website.com", "http://my.tokenissuer.com", 0.1d); // short lifetime

        Thread.Sleep(250);

        // act & assert
        Assert.Throws<SecurityTokenExpiredException>(() => 
            service.Decode(token, _validAudiences, _validIssuers));
    }
}
