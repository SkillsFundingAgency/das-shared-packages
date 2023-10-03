using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.GovUK.Auth.Extensions;

namespace SFA.DAS.GovUK.Auth.UnitTests.Extensions
{
    public class WhenGettingEnvironmentAndDomain
    {
        [Test, AutoData]
        public void Then_If_RedirectUri_Is_Not_Null_And_Environment_Not_Local_Then_Returned(string redirectUri, string environment)
        {
            var actual = redirectUri.GetEnvironmentAndDomain(environment);

            actual.Should().Be(redirectUri);
        }

        [Test, AutoData]
        public void Then_If_Local_Environment_Then_Empty_String_Returned(string redirectUri)
        {
            var actual = redirectUri.GetEnvironmentAndDomain("LocAL");

            actual.Should().BeEmpty();
        }

        [TestCase("Test", "test-eas.apprenticeships.education.gov.uk")]
        [TestCase("PROD", "prod-eas.apprenticeships.education.gov.uk")]
        [TestCase("preprod", "preprod-eas.apprenticeships.education.gov.uk")]
        public void Then_If_Redirect_Not_Supplied_Then_Url_Returned_For_Environment(string environment, string expectedUrl)
        {
            var actual = "".GetEnvironmentAndDomain(environment);

            actual.Should().Be(expectedUrl);
        }
    }    
}
