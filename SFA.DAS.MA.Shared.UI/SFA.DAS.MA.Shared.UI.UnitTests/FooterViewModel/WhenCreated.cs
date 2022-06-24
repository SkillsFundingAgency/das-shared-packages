using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.MA.Shared.UI.Configuration;
using SFA.DAS.MA.Shared.UI.Models;
using SFA.DAS.MA.Shared.UI.Models.Links;
using System;
using System.Linq;

namespace SFA.DAS.MA.Shared.UI.UnitTests.FooterViewModel
{
    [TestFixture]
    public class WhenCreated
    {
        private IFooterViewModel _sut;
        private Mock<IFooterConfiguration> _mockFooterConfiguration;
        private Mock<IUserContext> _mockUserContext;

        [SetUp]
        public void Setup()
        {
            _mockFooterConfiguration = new Mock<IFooterConfiguration>();
            _mockUserContext = new Mock<IUserContext>();
        }
       
        [Test]
        public void ThenHelpFooterLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";

            _mockFooterConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            // act
            _sut = new Models.FooterViewModel(_mockFooterConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<Help>().Where(t => t.Href.Equals($"{testManageApprenticeshipsBaseUrl}/service/help")).Count().Should().Be(1);
        }

        [Test]
        public void ThenFeedbackFooterLinkIsInitialised()
        {
            // arrange
            // act
            _sut = new Models.FooterViewModel(_mockFooterConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<Feedback>().Where(t => t.Href.Equals("https://www.smartsurvey.co.uk/s/apprenticeshipservicefeedback/")).Count().Should().Be(1);
        }

        [Test]
        public void ThenBuiltByFooterLinkIsInitialised()
        {
            // arrange
            // act
            _sut = new Models.FooterViewModel(_mockFooterConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<BuiltBy>().Where(t => t.Href.Equals("http://gov.uk/esfa")).Count().Should().Be(1);
        }

        [Test]
        public void ThenOpenGovernmentLicenseFooterLinkIsInitialised()
        {
            // arrange
            // act
            _sut = new Models.FooterViewModel(_mockFooterConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<OpenGovernmentLicense>().Where(t => t.Href.Equals("https://www.nationalarchives.gov.uk/doc/open-government-licence/version/3/")).Count().Should().Be(1);
        }

        [Test]
        public void ThenOpenGovernmentLicenseV3FooterLinkIsInitialised()
        {
            // arrange
            // act
            _sut = new Models.FooterViewModel(_mockFooterConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<OpenGovernmentLicenseV3>().Where(t => t.Href.Equals("https://www.nationalarchives.gov.uk/doc/open-government-licence/version/3/")).Count().Should().Be(1);
        }

        [Test]
        public void ThenCrownCopyrightFooterLinkIsInitialised()
        {
            // arrange
            // act
            _sut = new Models.FooterViewModel(_mockFooterConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<CrownCopyright>().Where(t => t.Href.Equals("https://www.nationalarchives.gov.uk/information-management/re-using-public-sector-information/uk-government-licensing-framework/crown-copyright/")).Count().Should().Be(1);
        }

        [Test]
        public void ThenPrivacyFooterLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";
            var testHashedAccountId = Guid.NewGuid().ToString();

            _mockFooterConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            _mockUserContext
                .Setup(m => m.HashedAccountId)
                .Returns(testHashedAccountId);

            // act
            _sut = new Models.FooterViewModel(_mockFooterConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<Privacy>().Where(t => t.Href.Equals($"{testManageApprenticeshipsBaseUrl}/service/{testHashedAccountId}/privacy")).Count().Should().Be(1);
        }

        [Test]
        public void ThenWhenUserContextHasAHashedAccountIdTheCookiesFooterLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";
            var testHashedAccountId = Guid.NewGuid().ToString();

            _mockFooterConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            _mockUserContext
                .Setup(m => m.HashedAccountId)
                .Returns(testHashedAccountId);

            // act
            _sut = new Models.FooterViewModel(_mockFooterConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<Cookies>().Where(t => t.Href.Equals($"{testManageApprenticeshipsBaseUrl}/accounts/{testHashedAccountId}/cookieConsent")).Count().Should().Be(1);
        }

        [Test]
        public void ThenWhenUserContextDoesNotHaveAHashedAccountIdTheCookiesFooterLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";

            _mockFooterConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            // act
            _sut = new Models.FooterViewModel(_mockFooterConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<Cookies>().Where(t => t.Href.Equals($"{testManageApprenticeshipsBaseUrl}/cookieConsent")).Count().Should().Be(1);
        }

        [Test]
        public void ThenTermsAndConditionsFooterLinkIsInitialised()
        {
            // arrange
            var testAuthenticationAuthorityUrl = $"http://{Guid.NewGuid()}";

            _mockFooterConfiguration
                .Setup(m => m.AuthenticationAuthorityUrl)
                .Returns(testAuthenticationAuthorityUrl);

            // act
            _sut = new Models.FooterViewModel(_mockFooterConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<TermsAndConditions>().Where(t => t.Href.Equals($"{testAuthenticationAuthorityUrl}/TermsAndConditions")).Count().Should().Be(1);
        }

    }
}
