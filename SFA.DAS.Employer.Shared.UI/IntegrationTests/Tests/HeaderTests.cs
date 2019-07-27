using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using DfE.Example.Web;
using SFA.DAS.Employer.Shared.UI.IntegrationTests.Helpers;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests
{
    public class HeaderTests : TestBase
    {
        public HeaderTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task Header_Links()
        {
            // Arrange
            var client = BuildClient();

            // Act
            var response = await client.GetAsync("/ABC123");
            var content = await HtmlHelpers.GetDocumentAsync(response);
            var headerLink = content.QuerySelector(".mu-header");

            // Assert
            Assert.Equal("https://accounts.at-eas.apprenticeships.education.gov.uk", headerLink.Attributes["href"].Value); 
        }
    }
}
