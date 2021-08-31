using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using DfE.Example.Web;
using SFA.DAS.Employer.Shared.UI.IntegrationTests.Helpers;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests
{
    public class MenuTests : TestBase
    {
        public MenuTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task AccountActions_Links()
        {
            // Arrange
            var client = BuildClient();

            // Act
            var response = await client.GetAsync("/ABC123");
            var content = await HtmlHelpers.GetDocumentAsync(response);
            var helpLink = content.QuerySelector(".mu-help");
            var accountsLink = content.QuerySelector("a.mu-accounts");
            var renameAccLink = content.QuerySelector("a.mu-rename-acc");
            var changePassLink = content.QuerySelector("a.mu-change-pass");
            var changeEmail = content.QuerySelector("a.mu-change-email");
            var notificationSettingsLink = content.QuerySelector("a.mu-not-settings");
            var advertNotificationsLink = content.QuerySelector("a.mu-advert-notifications");

            // Assert
            Assert.Equal("https://accounts.at-eas.apprenticeships.education.gov.uk/service/help", helpLink.Attributes["href"].Value, ignoreCase: true); 
            Assert.Equal("https://accounts.at-eas.apprenticeships.education.gov.uk/service/accounts", accountsLink.Attributes["href"].Value, ignoreCase: true); 
            Assert.Equal("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/rename", renameAccLink.Attributes["href"].Value, ignoreCase: true); 
            Assert.Equal("https://at-login.apprenticeships.education.gov.uk/account/changepassword?clientId=easaccdev&returnurl=https%3a%2f%2faccounts.at-eas.apprenticeships.education.gov.uk%2fservice%2fpassword%2fchange", changePassLink.Attributes["href"].Value, ignoreCase: true); 
            Assert.Equal("https://at-login.apprenticeships.education.gov.uk/account/changeemail?clientId=easaccdev&returnurl=https%3a%2f%2faccounts.at-eas.apprenticeships.education.gov.uk%2fservice%2femail%2fchange", changeEmail.Attributes["href"].Value, ignoreCase: true); 
            Assert.Equal("https://accounts.at-eas.apprenticeships.education.gov.uk/settings/notifications", notificationSettingsLink.Attributes["href"].Value, ignoreCase: true);
            Assert.Equal("https://recruit.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/notifications-manage", notificationSettingsLink.Attributes["href"].Value, ignoreCase: true);
        }
        
        [Fact]
        public async Task AccountActions_Signin()
        {
            // Arrange
            var client = BuildClient(authenticated: false);

            // Act
            var response = await client.GetAsync("/ABC123");
            var content = await HtmlHelpers.GetDocumentAsync(response);
            var signinLink = content.QuerySelector(".mu-signin");

            // Assert
            Assert.Equal("https://accounts.at-eas.apprenticeships.education.gov.uk", signinLink.Attributes["href"].Value, ignoreCase: true); 
        }

        [Fact]
        public async Task MenuItem_Links()
        {
            // Arrange
            var client = BuildClient();

            // Act
            var response = await client.GetAsync("/ABC123");
            var content = await HtmlHelpers.GetDocumentAsync(response);
            var homeLink = content.QuerySelector(".mu-home");
            var financeLink = content.QuerySelector("a.mu-finance");
            var recruitLink = content.QuerySelector("a.mu-recruit");
            var apprenticesLink = content.QuerySelector("a.mu-apprentices");
            var yourTeamLink = content.QuerySelector("a.mu-team");
            var orgsLink = content.QuerySelector("a.mu-orgs");
            var payeLink = content.QuerySelector("a.mu-paye");

            // Assert
            Assert.Equal("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/teams", homeLink.Attributes["href"].Value, ignoreCase: true); 
            Assert.Equal("https://finance.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/finance", financeLink.Attributes["href"].Value, ignoreCase: true); 
            Assert.Equal("https://recruit.at-eas.apprenticeships.education.gov.uk/accounts/ABC123", recruitLink.Attributes["href"].Value, ignoreCase: true); 
            Assert.Equal("https://approvals.at-eas.apprenticeships.education.gov.uk/ABC123", apprenticesLink.Attributes["href"].Value, ignoreCase: true); 
            Assert.Equal("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/teams/view", yourTeamLink.Attributes["href"].Value, ignoreCase: true); 
            Assert.Equal("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/agreements", orgsLink.Attributes["href"].Value, ignoreCase: true); 
            Assert.Equal("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/schemes", payeLink.Attributes["href"].Value, ignoreCase: true);
        }
    }
}
