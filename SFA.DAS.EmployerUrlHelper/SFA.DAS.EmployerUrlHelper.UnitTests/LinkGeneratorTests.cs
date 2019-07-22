using Moq;
using NUnit.Framework;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.EmployerUrlHelper.Configuration;

namespace SFA.DAS.EmployerUrlHelper.UnitTests
{
    [TestFixture]
    public class LinkGeneratorTests
    {
        [Test]
        public void Link_WhenBaseUrlAndPathAreNotTrimmed_ShouldReturnTrimmedUrl()
        {
            var autoConfigurationService = new Mock<IAutoConfigurationService>();
            
            var employerUrlHelperConfiguration = new EmployerUrlHelperConfiguration
            {
                AccountsBaseUrl = "https://accounts/",
                CommitmentsBaseUrl = "https://commitments/",
                CommitmentsV2BaseUrl = "https://commitmentsv2/",
                PortalBaseUrl = "https://portal/",
                ProjectionsBaseUrl = "https://projections/",
                RecruitBaseUrl = "https://recruit/",
                UsersBaseUrl = "https://users/"
            };

            autoConfigurationService.Setup(s => s.Get<EmployerUrlHelperConfiguration>()).Returns(employerUrlHelperConfiguration);
            
            var linkGenerator = new LinkGenerator(autoConfigurationService.Object);
            
            Assert.AreEqual("https://accounts/path", linkGenerator.AccountsLink("/path/"));
            Assert.AreEqual("https://commitments/path", linkGenerator.CommitmentsLink("/path/"));
            Assert.AreEqual("https://commitmentsv2/path", linkGenerator.CommitmentsV2Link("/path/"));
            Assert.AreEqual("https://portal/path", linkGenerator.PortalLink("/path/"));
            Assert.AreEqual("https://projections/path", linkGenerator.ProjectionsLink("/path/"));
            Assert.AreEqual("https://recruit/path", linkGenerator.RecruitLink("/path/"));
            Assert.AreEqual("https://users/path", linkGenerator.UsersLink("/path/"));
        }
    }
}