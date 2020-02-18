using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.MA.Shared.UI.Configuration;
using SFA.DAS.MA.Shared.UI.Models;
using SFA.DAS.MA.Shared.UI.Models.Links;
using System;
using System.Linq;

namespace SFA.DAS.MA.Shared.UI.UnitTest.HeaderViewModel
{
    [TestFixture]
    public class WhenCreated
    {
        private IHeaderViewModel _sut;
        private Mock<IHeaderConfiguration> _mockHeaderConfiguration;
        private Mock<IUserContext> _mockUserContext;
        private Mock<IAuthorizationService> _mockAuthorizationService;

        [SetUp]
        public void Setup()
        {
            _mockHeaderConfiguration = new Mock<IHeaderConfiguration>();
            _mockUserContext = new Mock<IUserContext>();
            _mockAuthorizationService = new Mock<IAuthorizationService>();
        }

        [Test]
        public void ThenGovUkHeaderLinkIsInitialised()
        {
            // arrange
            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<GovUk>().Where(t => t.Href.Equals("https://www.gov.uk/")).Count().Should().Be(1);
        }

        [Test]
        public void ThenManageApprenticeshipsHeaderLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";

            _mockHeaderConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<ManageApprenticeships>().Where(t => t.Href.Equals(testManageApprenticeshipsBaseUrl)).Count().Should().Be(1);
        }

        [Test]
        public void ThenHelpHeaderLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";

            _mockHeaderConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<Help>().Where(t => t.Href.Equals($"{testManageApprenticeshipsBaseUrl}/service/help")).Count().Should().Be(1);
        }

        [Test]
        public void ThenYourAccountsHeaderLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";

            _mockHeaderConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<YourAccounts>().Where(t => t.Href.Equals($"{testManageApprenticeshipsBaseUrl}/service/accounts")).Count().Should().Be(1);
        }

        [Test]
        public void ThenRenameAccountHeaderLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";

            _mockHeaderConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<RenameAccount>().Where(t => t.Href.Equals($"{testManageApprenticeshipsBaseUrl}/accounts/rename")).Count().Should().Be(1);
        }

        [Test]
        public void ThenChangePassswordHeaderLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";
            var testAuthenticationAuthorityUrl = $"http://{Guid.NewGuid()}";
            var testClientId = Guid.NewGuid().ToString();
            var returnUrl = System.Net.WebUtility.UrlEncode($"{testManageApprenticeshipsBaseUrl}/service/password/change");

            _mockHeaderConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            _mockHeaderConfiguration
                .Setup(m => m.AuthenticationAuthorityUrl)
                .Returns(testAuthenticationAuthorityUrl);

            _mockHeaderConfiguration
                .Setup(m => m.ClientId)
                .Returns(testClientId);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<ChangePassword>().Where(t => t.Href.Equals($"{testAuthenticationAuthorityUrl}/account/changepassword?clientId={testClientId}&returnurl={returnUrl}")).Count().Should().Be(1);
        }

        [Test]
        public void ThenChangeEmailHeaderLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";
            var testAuthenticationAuthorityUrl = $"http://{Guid.NewGuid()}";
            var testClientId = Guid.NewGuid().ToString();
            var returnUrl = System.Net.WebUtility.UrlEncode($"{testManageApprenticeshipsBaseUrl}/service/email/change");

            _mockHeaderConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            _mockHeaderConfiguration
                .Setup(m => m.AuthenticationAuthorityUrl)
                .Returns(testAuthenticationAuthorityUrl);

            _mockHeaderConfiguration
                .Setup(m => m.ClientId)
                .Returns(testClientId);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<ChangeEmail>().Where(t => t.Href.Equals($"{testAuthenticationAuthorityUrl}/account/changeemail?clientId={testClientId}&returnurl={returnUrl}")).Count().Should().Be(1);
        }

        [Test]
        public void ThenNotificationSettingsHeaderLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";

            _mockHeaderConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<NotificationSettings>().Where(t => t.Href.Equals($"{testManageApprenticeshipsBaseUrl}/settings/notifications")).Count().Should().Be(1);
        }

        [Test]
        public void ThenSignOutHeaderLinkIsInitialised()
        {
            // arrange
            var testSignoutUrl = new Uri($"http://{Guid.NewGuid()}");

            _mockHeaderConfiguration
                .Setup(m => m.SignOutUrl)
                .Returns(testSignoutUrl);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<SignOut>().Where(t => t.Href.Equals(testSignoutUrl.AbsoluteUri)).Count().Should().Be(1);
        }

        [Test]
        public void ThenSignInHeaderLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";

            _mockHeaderConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<SignIn>().Where(t => t.Href.Equals($"{testManageApprenticeshipsBaseUrl}/service/signIn")).Count().Should().Be(1);
        }

        [Test]
        public void ThenHomeHeaderLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";
            var testHashedAccountId = Guid.NewGuid().ToString();

            _mockHeaderConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            _mockUserContext
                .Setup(m => m.HashedAccountId)
                .Returns(testHashedAccountId);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<Home>().Where(t => t.Href.Equals($"{testManageApprenticeshipsBaseUrl}/accounts/{testHashedAccountId}/teams")).Count().Should().Be(1);
        }

        [Test]
        public void ThenFinanceHeaderLinkIsInitialised()
        {
            // arrange
            var testEmployerFinanceBaseUrl = $"http://{Guid.NewGuid()}";
            var testHashedAccountId = Guid.NewGuid().ToString();

            _mockHeaderConfiguration
                .Setup(m => m.EmployerFinanceBaseUrl)
                .Returns(testEmployerFinanceBaseUrl);

            _mockUserContext
                .Setup(m => m.HashedAccountId)
                .Returns(testHashedAccountId);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<Finance>().Where(t => t.Href.Equals($"{testEmployerFinanceBaseUrl}/accounts/{testHashedAccountId}/finance")).Count().Should().Be(1);
        }

        [Test]
        public void ThenRecruitmentHeaderLinkIsInitialised()
        {
            // arrange
            var testEmployerRecruitBaseUrl = $"http://{Guid.NewGuid()}";
            var testHashedAccountId = Guid.NewGuid().ToString();

            _mockHeaderConfiguration
                .Setup(m => m.EmployerRecruitBaseUrl)
                .Returns(testEmployerRecruitBaseUrl);

            _mockHeaderConfiguration
               .Setup(m => m.AuthorizationService)
               .Returns(_mockAuthorizationService.Object);

            _mockUserContext
                .Setup(m => m.HashedAccountId)
                .Returns(testHashedAccountId);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<Recruitment>().Where(t => t.Href.Equals($"{testEmployerRecruitBaseUrl}/accounts/{testHashedAccountId}")).Count().Should().Be(1);
        }

        [Test]
        public void ThenApprenticesHeaderLinkIsInitialised()
        {
            // arrange
            var testEmployerCommitmentsBaseUrl = $"http://{Guid.NewGuid()}";
            var testHashedAccountId = Guid.NewGuid().ToString();

            _mockHeaderConfiguration
                .Setup(m => m.EmployerCommitmentsBaseUrl)
                .Returns(testEmployerCommitmentsBaseUrl);

            _mockUserContext
                .Setup(m => m.HashedAccountId)
                .Returns(testHashedAccountId);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<Apprentices>().Where(t => t.Href.Equals($"{testEmployerCommitmentsBaseUrl}/accounts/{testHashedAccountId}/apprentices/home")).Count().Should().Be(1);
        }

        [Test]
        public void ThenYourTeamHeaderLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";
            var testHashedAccountId = Guid.NewGuid().ToString();

            _mockHeaderConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            _mockUserContext
                .Setup(m => m.HashedAccountId)
                .Returns(testHashedAccountId);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<YourTeam>().Where(t => t.Href.Equals($"{testManageApprenticeshipsBaseUrl}/accounts/{testHashedAccountId}/teams/view")).Count().Should().Be(1);
        }

        [Test]
        public void ThenYourOrganisationsHeaderLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";
            var testHashedAccountId = Guid.NewGuid().ToString();

            _mockHeaderConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            _mockUserContext
                .Setup(m => m.HashedAccountId)
                .Returns(testHashedAccountId);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<YourOrganisations>().Where(t => t.Href.Equals($"{testManageApprenticeshipsBaseUrl}/accounts/{testHashedAccountId}/agreements")).Count().Should().Be(1);
        }

        [Test]
        public void ThenPayeSchemesHeaderLinkIsInitialised()
        {
            // arrange
            var testManageApprenticeshipsBaseUrl = $"http://{Guid.NewGuid()}";
            var testHashedAccountId = Guid.NewGuid().ToString();

            _mockHeaderConfiguration
                .Setup(m => m.ManageApprenticeshipsBaseUrl)
                .Returns(testManageApprenticeshipsBaseUrl);

            _mockUserContext
                .Setup(m => m.HashedAccountId)
                .Returns(testHashedAccountId);

            // act
            _sut = new Models.HeaderViewModel(_mockHeaderConfiguration.Object, _mockUserContext.Object);

            // assert            
            _sut.Links.OfType<PayeSchemes>().Where(t => t.Href.Equals($"{testManageApprenticeshipsBaseUrl}/accounts/{testHashedAccountId}/schemes")).Count().Should().Be(1);
        }
    }
}
