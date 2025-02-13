using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Employer.Shared.UI.IntegrationTests.Helpers;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests.Tests;

public class HeaderTests : TestBase
{
    [Test]
    public async Task Header_Links()
    {
        // Arrange
        var client = BuildClient();

        // Act
        var response = await client.GetAsync("/ABC123");
        var content = await HtmlHelpers.GetDocumentAsync(response);
        var headerLink = content.QuerySelector(".mu-header");

        // Assert
        headerLink.Attributes["href"].Value.Should().Be("https://accounts.at-eas.apprenticeships.education.gov.uk/"); 
    }
}