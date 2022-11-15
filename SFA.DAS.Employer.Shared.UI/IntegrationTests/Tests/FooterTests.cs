using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Employer.Shared.UI.IntegrationTests.Helpers;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests
{
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

            // Assert
            Assert.AreEqual("https://accounts.at-eas.apprenticeships.education.gov.uk/service/help", helpLink.Attributes["href"].Value); 
            Assert.AreEqual("https://accounts.at-eas.apprenticeships.education.gov.uk/service/ABC123/privacy", privacyLink.Attributes["href"].Value);
            Assert.AreEqual("https://accounts.at-eas.apprenticeships.education.gov.uk/accounts/ABC123/cookieConsent", cookieConsent.Attributes["href"].Value);
            Assert.AreEqual("https://accounts.at-eas.apprenticeships.education.gov.uk/service/termsAndConditions/overview", termsOfUse.Attributes["href"].Value);
        }
    }
}
