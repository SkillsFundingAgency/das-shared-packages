using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;

namespace SFA.DAS.Authentication.Extensions.UnitTest.DasJwtSecurityTokenHandler
{
    [TestFixture]
    public class WhenValidateToken
    {
        private Mock<ILog> _mockLog;
        private Extensions.DasJwtSecurityTokenHandler _sut;
        private TokenValidationParameters _tokenValidationParameters;
        private string _securityToken;

        [SetUp]
        public void Setup()
        {
            _mockLog = new Mock<ILog>();
            _mockLog.Setup(m => m.Error(It.IsAny<Exception>(), It.IsAny<string>())).Verifiable();

            _sut = new Extensions.DasJwtSecurityTokenHandler(_mockLog.Object);
            _tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new InMemorySymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("secretsauceonitsownisnotlongenough")),
                ValidIssuers = new[] { "http://example.com" },
                ValidAudiences = new[] { "doubleauthpoc" }
            };
            _securityToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJkYXRhIjoiUm9sZTEiLCJpc3MiOiJodHRwOi8vZXhhbXBsZS5jb20iLCJhdWQiOiJkb3VibGVhdXRocG9jIiwiZXhwIjoxNTkwOTI3MTYwLCJuYmYiOjE1NTg1MjcxNjB9.y3547L3UyGby-EzGBWO4IQAX16wVQA0FnrwQHltlXdA";
        }

        [Test]
        public void ThenRolesInThedataClaimAreReturnedAsSeparateClaimsInTheClaimsPrincipal()
        {
            // arrange
            // act
            var result = _sut.ValidateToken(_securityToken, _tokenValidationParameters, out SecurityToken validatedtoken);

            // assert            
            result.Should().NotBeNull();
            result.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Role)).Should().NotBe(null);
            result.Claims.Single(c => c.Type.Equals(ClaimTypes.Role)).Value.Should().Be("Role1");
        }

        [Test]
        public void ThenThrowsExceptionWithInvalidIssuer()
        {
            // arrange
            _tokenValidationParameters.ValidIssuers = new[] { "http://invalid.com" };

            // act
            Action act = () => _sut.ValidateToken(_securityToken, _tokenValidationParameters, out SecurityToken validatedtoken);

            // assert            
            act.Should().Throw<SecurityTokenInvalidIssuerException>();
        }

        [Test]
        public void ThenLogsErrorWithInvalidIssuer()
        {
            // arrange
            _tokenValidationParameters.ValidIssuers = new[] { "http://invalid.com" };

            // act
            try
            {
                _sut.ValidateToken(_securityToken, _tokenValidationParameters, out SecurityToken validatedtoken);
            }
            catch { }

            // assert
            _mockLog.Verify(m =>
            m.Error(
                It.Is<Exception>(e => e.Message.Contains("IDX10205: Issuer validation failed")),
                It.Is<string>(s => s.Equals("Failed to validate the security token"))),
                Times.Once);
        }

        [Test]
        public void ThenThrowsExceptionWithInvalidAudience()
        {
            // arrange
            _tokenValidationParameters.ValidAudiences = new[] { "invalid" };

            // act
            Action act = () => _sut.ValidateToken(_securityToken, _tokenValidationParameters, out SecurityToken validatedtoken);

            // assert            
            act.Should().Throw<SecurityTokenInvalidAudienceException>();
        }

        [Test]
        public void ThenLogsErrorWithInvalidAudience()
        {
            // arrange
            _tokenValidationParameters.ValidAudiences = new[] { "invalid" };

            // act
            try
            {
                _sut.ValidateToken(_securityToken, _tokenValidationParameters, out SecurityToken validatedtoken);
            }
            catch { }

            // assert    
            _mockLog.Verify(m =>
            m.Error(
                It.Is<Exception>(e => e.Message.Contains("IDX10214: Audience validation failed")),
                It.Is<string>(s => s.Equals("Failed to validate the security token"))),
                Times.Once);
        }

        [Test]
        public void ThenThrowsExceptionWithInvalidPrivateKey()
        {
            // arrange
            _tokenValidationParameters.IssuerSigningKey = new InMemorySymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("invalid"));

            // act
            Action act = () => _sut.ValidateToken(_securityToken, _tokenValidationParameters, out SecurityToken validatedtoken);

            // assert            
            act.Should().Throw<Exception>();
        }

        [Test]
        public void ThenLogsErrorWithInvalidPrivateKey()
        {
            // arrange
            _tokenValidationParameters.IssuerSigningKey = new InMemorySymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("invalid"));

            // act
            try
            {
                _sut.ValidateToken(_securityToken, _tokenValidationParameters, out SecurityToken validatedtoken);
            }
            catch { }

            // assert
            _mockLog.Verify(m =>
            m.Error(
                It.Is<Exception>(e => e.Message.Contains("IDX10503: Signature validation failed")),
                It.Is<string>(s => s.Equals("Failed to validate the security token"))),
                Times.Once);
        }
    }
}
