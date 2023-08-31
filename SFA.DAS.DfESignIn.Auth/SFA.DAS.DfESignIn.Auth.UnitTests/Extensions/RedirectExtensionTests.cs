using FluentAssertions;
using SFA.DAS.DfESignIn.Auth.Extensions;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Extensions
{
    public class RedirectExtensionTests
    {
        [Test]
        public void Then_The_Domain_Is_Resolved_For_Local_Environment()
        {
            var actual = RedirectExtension.GetEnvironmentAndDomain("LOcal");

            actual.Should().BeEmpty();
        }

        [TestCase("test")]
        [TestCase("pp")]
        [TestCase("something")]
        public void Then_The_Domain_Is_Resolved_For_Test_Environments(string environment)
        {
            var actual = RedirectExtension.GetEnvironmentAndDomain(environment);

            actual.Should().Be($"{environment}-pas.apprenticeships.education.gov.uk");
        }

        [Test]
        public void Then_The_Domain_Is_Resolved_For_Prod_Environment()
        {
            var actual = RedirectExtension.GetEnvironmentAndDomain("PRD");

            actual.Should().Be("providers.apprenticeships.education.gov.uk");
        }
        
        
        [Test]
        public void Then_The_Logged_Out_Url_Is_Returned_When_Specified()
        {
            var actual = "test".GetSignedOutRedirectUrl("environment");

            actual.Should().Be("test");
        }

        [TestCase("test")]
        [TestCase("pp")]
        [TestCase("something")]
        public void Then_The_Logged_Out_Url_Is_Returned_For_Test(string environment)
        {
            var actual = "".GetSignedOutRedirectUrl(environment);
            
            actual.Should().Be($"https://{environment}-pas.apprenticeships.education.gov.uk");
        }
        
        [TestCase("test")]
        [TestCase("pp")]
        [TestCase("something")]
        public void Then_The_Logged_Out_Url_Is_Returned_For_Prod(string environment)
        {
            var actual = "".GetSignedOutRedirectUrl("prd");
            
            actual.Should().Be("https://providers.apprenticeships.education.gov.uk");
        }
    }
}