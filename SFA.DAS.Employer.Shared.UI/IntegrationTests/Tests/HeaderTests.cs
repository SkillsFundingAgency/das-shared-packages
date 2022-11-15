using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Employer.Shared.UI.IntegrationTests.Helpers;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests
{
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
            Assert.AreEqual("https://accounts.at-eas.apprenticeships.education.gov.uk/", headerLink.Attributes["href"].Value); 
        }
    }
}
