using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Employer.Shared.UI.IntegrationTests.Helpers;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests.Tests;

public class FooterTests : TestBase
{
    [Test]
    public async Task Footer_Links()
    {
        // Arrange
        var client = BuildClient();

        // Act
        var response = await client.GetAsync("/ABC123");
        var content = await HtmlHelpers.GetDocumentAsync(response);
        var helpLink = content.QuerySelector(".mu-foot-help");
        var privacyLink = content.QuerySelector(".mu-foot-privacy");
        var cookieConsent = content.QuerySelector(".mu-foot-cookieConsent");
        var termsOfUse = content.QuerySelector(".mu-foot-termsofuse");
        var accessibilityStatement = content.QuerySelector(".mu-foot-accessibilityStatement");

        // Assert
        helpLink.Attributes["href"].Value.Should().Be("https://accounts.at-eas.apprenticeships.education.gov.uk/service/help");
        privacyLink.Attributes["href"].Value.Should().Be("https://accounts.at-eas.apprenticeships.education.gov.uk/service/ABC123/privacy");
        cookieConsent.Attributes["href"].Value.Should().Be("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/cookieConsent");
        termsOfUse.Attributes["href"].Value.Should().Be("https://accounts.at-eas.apprenticeships.education.gov.uk/service/termsAndConditions/overview");
        accessibilityStatement.Attributes["href"].Value.Should().Be("https://accounts.at-eas.apprenticeships.education.gov.uk/service/accessibility-statement");
    }
}