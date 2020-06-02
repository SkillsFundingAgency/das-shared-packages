using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.MA.Shared.UI.Configuration;
using SFA.DAS.MA.Shared.UI.Models;
using SFA.DAS.MA.Shared.UI.Services;

namespace SFA.DAS.MA.Shared.UI.UnitTest.CookieBannerViewModel
{
    [TestFixture]
    public class WhenCreated
    {
        private ICookieBannerViewModel _sut;
        private Mock<IFooterConfiguration> _mockFooterConfiguration;
        private Mock<IUserContext> _mockUserContext;

        [SetUp]
        public void Setup()
        {
            _mockFooterConfiguration = new Mock<IFooterConfiguration>();
            _mockUserContext = new Mock<IUserContext>();
        }

        [Test]
        public void ThenCookieConsentUrlIsInitialised()
        {
            // arrange
            var cookieConsentPath = "cookieConsent";
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";

            _mockFooterConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            // act
            _sut = new Models.CookieBannerViewModel(_mockFooterConfiguration.Object, _mockUserContext.Object, new UrlHelper());

            // assert  
            Assert.AreEqual($"{testManageApprenticeshipsBaseUrl}/{cookieConsentPath}", _sut.CookieConsentUrl);
        }

        [Test]
        public void ThenCookieDetailsUrlIsInitialised()
        {
            // arrange
            var cookieDetailsPath = "cookieConsent/details";
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";

            _mockFooterConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            // act
            _sut = new Models.CookieBannerViewModel(_mockFooterConfiguration.Object, _mockUserContext.Object, new UrlHelper());

            // assert  
            Assert.AreEqual($"{testManageApprenticeshipsBaseUrl}/{cookieDetailsPath}", _sut.CookieDetailsUrl);
        }
    }
}
