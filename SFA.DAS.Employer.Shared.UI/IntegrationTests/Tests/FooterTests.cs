using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using DfE.Example.Web;
using SFA.DAS.Employer.Shared.UI.IntegrationTests.Helpers;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests
{
    public class FooterTests : TestBase
    {
        public FooterTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Footer_Links()
        {
            // Arrange
            var client = BuildClient();

            // Act
            var response = await client.GetAsync("/ABC123");
            var content = await HtmlHelpers.GetDocumentAsync(response);
            var helpLink = content.QuerySelector(".mu-foot-help");
            var privacyLink = content.QuerySelector(".mu-foot-privacy");

            // Assert
            Assert.Equal("https://accounts.at-eas.apprenticeships.education.gov.uk/service/help", helpLink.Attributes["href"].Value); 
            Assert.Equal("https://accounts.at-eas.apprenticeships.education.gov.uk/service/privacy", privacyLink.Attributes["href"].Value); 
        }
    }
}
