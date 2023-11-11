using FluentAssertions;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Extensions;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.Extensions
{
    public class RedirectExtensionTests
    {
        [Test]
        public void Then_The_Domain_Is_Resolved_For_Local_Environment()
        {
            var actual = RedirectExtension.GetEnvironmentAndDomain("LOcal", ClientName.ProviderRoatp);

            actual.Should().BeEmpty();
        }

        [TestCase("test")]
        [TestCase("pp")]
        [TestCase("something")]
        public void Then_The_Domain_Is_Resolved_For_Test_Environments(string environment)
        {
            var actual = RedirectExtension.GetEnvironmentAndDomain(environment, ClientName.ProviderRoatp);

            actual.Should().Be($"{environment}-pas.apprenticeships.education.gov.uk");
        }
        [TestCase("test")]
        [TestCase("pp")]
        [TestCase("something")]
        public void Then_The_Domain_Is_Resolved_For_Test_Environments_Traineeships(string environment)
        {
            var actual = RedirectExtension.GetEnvironmentAndDomain(environment, ClientName.TraineeshipRoatp);

            actual.Should().Be($"{environment}-pas.traineeships.education.gov.uk");
        }

        [Test]
        public void Then_The_Domain_Is_Resolved_For_Prod_Environment()
        {
            var actual = RedirectExtension.GetEnvironmentAndDomain("PRD", ClientName.ProviderRoatp);

            actual.Should().Be("providers.apprenticeships.education.gov.uk");
        }
        
        [Test]
        public void Then_The_Domain_Is_Resolved_For_Prod_Traineeships_Environment()
        {
            var actual = RedirectExtension.GetEnvironmentAndDomain("PRD", ClientName.TraineeshipRoatp);

            actual.Should().Be("providers.traineeships.education.gov.uk");
        }
        
        [Test]
        public void Then_The_Logged_Out_Url_Is_Returned_When_Specified()
        {
            var actual = "test".GetSignedOutRedirectUrl("environment", ClientName.ProviderRoatp);

            actual.Should().Be("test");
        }

        [TestCase("test","test-pas.", ClientName.ProviderRoatp)]
        [TestCase("pp","pp-pas.", ClientName.ProviderRoatp)]
        [TestCase("something","something-pas.", ClientName.ProviderRoatp)]
        [TestCase("test","test-admin.", ClientName.ServiceAdmin)]
        [TestCase("test","test-adminaan.", ClientName.ServiceAdminAan)]
        [TestCase("test","test-support-tools.", ClientName.BulkStop)]
        [TestCase("test","test-identify-data-locks.", ClientName.DataLocks)]
        [TestCase("test","test-review.", ClientName.Qa)]
        [TestCase("test","test-console.", ClientName.SupportConsole)]
        [TestCase("test","",ClientName.RoatpServiceAdmin)]
        public void Then_The_Logged_Out_Url_Is_Returned_For_Test(string environment,string expectedUrlPart, ClientName clientName)
        {
            var actual = RedirectExtension.GetEnvironmentAndDomain(environment, clientName);
            
            actual.Should().Be($"{expectedUrlPart}apprenticeships.education.gov.uk");
        }
        
        [TestCase(ClientName.ProviderRoatp, "providers.apprenticeships")]
        [TestCase(ClientName.ServiceAdmin, "admin.apprenticeships")]
        [TestCase(ClientName.ServiceAdminAan, "adminaan.apprenticeships")]
        [TestCase(ClientName.Qa, "review.apprenticeships")]
        [TestCase(ClientName.DataLocks, "identify-data-locks.apprenticeships")]
        [TestCase(ClientName.BulkStop, "support-tools.apprenticeships")]
        [TestCase(ClientName.SupportConsole, "console.apprenticeships")]
        [TestCase(ClientName.RoatpServiceAdmin, "admin.apprenticeships")]
        public void Then_The_Logged_Out_Url_Is_Returned_For_Prod_For_Each_Client_Type(ClientName clientName, string expectedUrlPart)
        {
            var actual = "".GetSignedOutRedirectUrl("prd", clientName);
            
            actual.Should().Be($"https://{expectedUrlPart}.education.gov.uk");
        }
    }
}