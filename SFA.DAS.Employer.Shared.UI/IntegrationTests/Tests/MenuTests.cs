using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Employer.Shared.UI.IntegrationTests.Helpers;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests
{
    public class MenuTests : TestBase
    {
        [Test]
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
            Assert.AreEqual("https://accounts.at-eas.apprenticeships.education.gov.uk/service/help", helpLink.Attributes["href"].Value); 
            Assert.AreEqual("https://accounts.at-eas.apprenticeships.education.gov.uk/service/accounts", accountsLink.Attributes["href"].Value); 
            Assert.AreEqual("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/rename", renameAccLink.Attributes["href"].Value); 
            Assert.IsTrue(changePassLink.Attributes["href"].Value.Equals("https://at-login.apprenticeships.education.gov.uk/account/changepassword?clientId=easaccdev&returnurl=https%3a%2f%2faccounts.at-eas.apprenticeships.education.gov.uk%2fservice%2fpassword%2fchange", StringComparison.CurrentCultureIgnoreCase)); 
            Assert.IsTrue(changeEmail.Attributes["href"].Value.Equals("https://at-login.apprenticeships.education.gov.uk/account/changeemail?clientId=easaccdev&returnurl=https%3a%2f%2faccounts.at-eas.apprenticeships.education.gov.uk%2fservice%2femail%2fchange", StringComparison.CurrentCultureIgnoreCase)); 
            Assert.AreEqual("https://accounts.at-eas.apprenticeships.education.gov.uk/settings/notifications", notificationSettingsLink.Attributes["href"].Value);
            Assert.AreEqual("https://recruit.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/notifications-manage", advertNotificationsLink.Attributes["href"].Value);
        }
        
        [Test]
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
            Assert.AreEqual("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/teams", homeLink.Attributes["href"].Value); 
            Assert.AreEqual("https://finance.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/finance", financeLink.Attributes["href"].Value); 
            Assert.AreEqual("https://recruit.at-eas.apprenticeships.education.gov.uk/accounts/ABC123", recruitLink.Attributes["href"].Value); 
            Assert.AreEqual("https://approvals.at-eas.apprenticeships.education.gov.uk/ABC123", apprenticesLink.Attributes["href"].Value); 
            Assert.AreEqual("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/teams/view", yourTeamLink.Attributes["href"].Value); 
            Assert.AreEqual("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/agreements", orgsLink.Attributes["href"].Value); 
            Assert.AreEqual("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/schemes", payeLink.Attributes["href"].Value);
        }
    }
}
