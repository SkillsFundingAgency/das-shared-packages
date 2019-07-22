using NUnit.Framework;

namespace SFA.DAS.EmployerUrlHelper.UnitTests
{
    [TestFixture]
    public class LinkGeneratorTests
    {
        [Test]
        public void Link()
        {
            var employerUrlConfiguration = new EmployerUrlConfiguration
            {
                AccountsBaseUrl = "https://accounts/",
                CommitmentsBaseUrl = "https://commitments/",
                CommitmentsV2BaseUrl = "https://commitmentsv2/",
                PortalBaseUrl = "https://portal/",
                ProjectionsBaseUrl = "https://projections/",
                RecruitBaseUrl = "https://recruit/",
                UsersBaseUrl = "https://users/"
            };
            
            var linkGenerator = new LinkGenerator(employerUrlConfiguration);
            
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