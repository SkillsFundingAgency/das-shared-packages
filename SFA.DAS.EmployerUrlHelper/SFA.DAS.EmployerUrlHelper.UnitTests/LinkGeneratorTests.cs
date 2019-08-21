using NUnit.Framework;
using SFA.DAS.EmployerUrlHelper.Configuration;

namespace SFA.DAS.EmployerUrlHelper.UnitTests
{
    [TestFixture]
    public class LinkGeneratorTests
    {
        [Test]
        public void Link_WhenBaseUrlAndPathAreNotTrimmed_ShouldReturnTrimmedUrl()
        {
            var employerUrlHelperConfiguration = new EmployerUrlHelperConfiguration
            {
                AccountsBaseUrl = "https://accounts/",
                CommitmentsBaseUrl = "https://commitments/",
                CommitmentsV2BaseUrl = "https://commitmentsv2/",
                FinanceBaseUrl = "https://finance/",
                PortalBaseUrl = "https://portal/",
                ProjectionsBaseUrl = "https://projections/",
                RecruitBaseUrl = "https://recruit/",
                ReservationsBaseUrl = "https://reservations/",
                UsersBaseUrl = "https://users/"
            };
            
            ILinkGenerator linkGenerator = new LinkGenerator(employerUrlHelperConfiguration);
            
            Assert.AreEqual("https://accounts/path", linkGenerator.AccountsLink("/path/"));
            Assert.AreEqual("https://commitments/path", linkGenerator.CommitmentsLink("/path/"));
            Assert.AreEqual("https://commitmentsv2/path", linkGenerator.CommitmentsV2Link("/path/"));
            Assert.AreEqual("https://finance/path", linkGenerator.FinanceLink("/path/"));
            Assert.AreEqual("https://portal/path", linkGenerator.PortalLink("/path/"));
            Assert.AreEqual("https://projections/path", linkGenerator.ProjectionsLink("/path/"));
            Assert.AreEqual("https://recruit/path", linkGenerator.RecruitLink("/path/"));
            Assert.AreEqual("https://reservations/path", linkGenerator.ReservationsLink("/path/"));
            Assert.AreEqual("https://users/path", linkGenerator.UsersLink("/path/"));
        }

        [Test]
        public void Link_WhenNoPathIsSpecified_ShouldReturnBaseUrl()
        {
            var employerUrlHelperConfiguration = new EmployerUrlHelperConfiguration
            {
                AccountsBaseUrl = "https://accounts/",
                CommitmentsBaseUrl = "https://commitments/",
                CommitmentsV2BaseUrl = "https://commitmentsv2/",
                FinanceBaseUrl = "https://finance/",
                PortalBaseUrl = "https://portal/",
                ProjectionsBaseUrl = "https://projections/",
                RecruitBaseUrl = "https://recruit/",
                ReservationsBaseUrl = "https://reservations/",
                UsersBaseUrl = "https://users/"
            };

            ILinkGenerator linkGenerator = new LinkGenerator(employerUrlHelperConfiguration);

            Assert.AreEqual("https://accounts", linkGenerator.AccountsLink());
            Assert.AreEqual("https://commitments", linkGenerator.CommitmentsLink());
            Assert.AreEqual("https://commitmentsv2", linkGenerator.CommitmentsV2Link());
            Assert.AreEqual("https://finance", linkGenerator.FinanceLink());
            Assert.AreEqual("https://portal", linkGenerator.PortalLink());
            Assert.AreEqual("https://projections", linkGenerator.ProjectionsLink());
            Assert.AreEqual("https://recruit", linkGenerator.RecruitLink());
            Assert.AreEqual("https://reservations", linkGenerator.ReservationsLink());
            Assert.AreEqual("https://users", linkGenerator.UsersLink());
        }
    }
}