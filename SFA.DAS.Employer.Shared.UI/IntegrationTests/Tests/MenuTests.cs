using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Employer.Shared.UI.IntegrationTests.Helpers;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests.Tests;

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
        helpLink.Attributes["href"].Value.Should().Be("https://accounts.at-eas.apprenticeships.education.gov.uk/service/help");
        accountsLink.Attributes["href"].Value.Should().Be("https://accounts.at-eas.apprenticeships.education.gov.uk/service/accounts");
        renameAccLink.Attributes["href"].Value.Should().Be("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/rename");
        changePassLink.Attributes["href"].Value.Should().BeEquivalentTo("https://at-login.apprenticeships.education.gov.uk/account/changepassword?clientId=easaccdev&returnurl=https%3a%2f%2faccounts.at-eas.apprenticeships.education.gov.uk%2fservice%2fpassword%2fchange");
        changeEmail.Attributes["href"].Value.Should().BeEquivalentTo("https://at-login.apprenticeships.education.gov.uk/account/changeemail?clientId=easaccdev&returnurl=https%3a%2f%2faccounts.at-eas.apprenticeships.education.gov.uk%2fservice%2femail%2fchange");
        notificationSettingsLink.Attributes["href"].Value.Should().Be("https://accounts.at-eas.apprenticeships.education.gov.uk/settings/notifications");
        advertNotificationsLink.Attributes["href"].Value.Should().Be("https://recruit.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/notifications-manage");
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
        homeLink.Attributes["href"].Value.Should().Be("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/teams");
        financeLink.Attributes["href"].Value.Should().Be("https://finance.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/finance");
        recruitLink.Attributes["href"].Value.Should().Be("https://recruit.at-eas.apprenticeships.education.gov.uk/accounts/ABC123");
        apprenticesLink.Attributes["href"].Value.Should().Be("https://approvals.at-eas.apprenticeships.education.gov.uk/ABC123");
        yourTeamLink.Attributes["href"].Value.Should().Be("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/teams/view");
        orgsLink.Attributes["href"].Value.Should().Be("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/agreements");
        payeLink.Attributes["href"].Value.Should().Be("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/schemes");
    }
}