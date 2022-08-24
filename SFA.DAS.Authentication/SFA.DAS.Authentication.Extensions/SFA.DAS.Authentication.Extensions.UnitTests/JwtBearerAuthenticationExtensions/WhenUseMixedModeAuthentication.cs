using NUnit.Framework;
using System;
using FluentAssertions;
using Owin;
using Moq;
using SFA.DAS.NLog.Logger;
using Microsoft.Owin.Testing;

namespace SFA.DAS.Authentication.Extensions.UnitTests.JwtBearerAuthenticationExtensions
{
    [TestFixture]
    public class WhenUseMixedModeAuthentication
    {
        private MixedModeAuthenticationOptions _options;
        private Mock<IAppBuilder> _appBuilder;
        private Mock<ILog> _logger;

        [SetUp]
        public void Setup()
        {            
            _logger = new Mock<ILog>();

            _options = new MixedModeAuthenticationOptions
            {
                ApiTokenSecret = Guid.NewGuid().ToString(),
                Logger = _logger.Object,
                MetadataEndpoint = "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration",
                ValidAudiences = new[] {"validAudiences"},
                ValidIssuers = new[] { "ValidIssuers" },
            };

            _appBuilder = new Mock<IAppBuilder>();            
        }

        [Test]
        public void ThenThrowsExceptionWhenAppBuilderIsNull()
        {
                // arrange

                // act
                Action act = () => Extensions.JwtBearerAuthenticationExtensions.UseMixedModeAuthentication(null, _options);

            // assert            
            act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\nParameter name: app");
        }

        [Test]
        public void ThenThrowsExceptionWhenOptionsIsNull()
        {
            // arrange

            // act
            Action act = () => Extensions.JwtBearerAuthenticationExtensions.UseMixedModeAuthentication(_appBuilder.Object, null);

            // assert            
            act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\nParameter name: options");
        }

        [Test]
        public void ThenTheAppIsConfiguredWithoutErrors()
        {
            var testServer = TestServer.Create(a => {
                a.UseMixedModeAuthentication(_options);
            });

            Assert.Pass();
        }
    }
}

