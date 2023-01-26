using NUnit.Framework;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests
{
    public class UrlBuilderTests
    {
        [Test]
        public void AccountsLinkWithNullAccountId_ReturnRouteLink()
        {         

            var urlBuilder = new UrlBuilder("test");
            
            Assert.AreEqual("https://accounts.test-eas.apprenticeships.education.gov.uk/", urlBuilder.AccountsLink()); 

            Assert.AreEqual("https://accounts.test-eas.apprenticeships.education.gov.uk/accounts/ABC123/teams/view", urlBuilder.AccountsLink("AccountsTeamsView", "ABC123")); 
        }

        [Test]
        public void Then_The_Commitments_Links_Are_Built()
        {
            var urlBuilder = new UrlBuilder("test");
            
            Assert.AreEqual("https://approvals.test-eas.apprenticeships.education.gov.uk/ABC123", urlBuilder.CommitmentsV2Link("ApprenticesHome", "ABC123")); 
        }

        [Test]
        public void Then_The_Finance_Links_Are_Built()
        {
            var urlBuilder = new UrlBuilder("test");
            
            Assert.AreEqual("https://finance.test-eas.apprenticeships.education.gov.uk/accounts/ABC123/finance", urlBuilder.FinanceLink("AccountsFinance", "ABC123")); 
        }

        [Test]
        public void Then_The_Recruit_Links_Are_Built()
        {
            var urlBuilder = new UrlBuilder("test");
            
            Assert.AreEqual("https://recruit.test-eas.apprenticeships.education.gov.uk/accounts/ABC123", urlBuilder.RecruitLink("RecruitHome", "ABC123")); 
        }

        [Test]
        public void Then_The_Employer_Profiles_Links_Are_Built()
        {
            var urlBuilder = new UrlBuilder("pp");

            Assert.AreEqual("https://employerprofiles.pp-eas.apprenticeships.education.gov.uk/accounts/ABC123/employer/change-sign-in-details", urlBuilder.EmployerProfiles("ChangeLoginDetails", "ABC123"));
        }
        
        [Test]
        public void Then_The_User_Links_Are_Built()
        {
            var urlBuilder = new UrlBuilder("test");
            
            Assert.AreEqual("https://test-login.apprenticeships.education.gov.uk/account/changepassword?clientId=ABC123&returnUrl=return-address", urlBuilder.UsersLink("ChangePassword", new [] {"ABC123", "return-address"})); 
        }

        [TestCase("at","at-eas.apprenticeships.education")]
        [TestCase("test","test-eas.apprenticeships.education")]
        [TestCase("test2","test2-eas.apprenticeships.education")]
        [TestCase("pp", "pp-eas.apprenticeships.education")]
        [TestCase("Mo", "mo-eas.apprenticeships.education")]
        [TestCase("Demo", "demo-eas.apprenticeships.education")]
        [TestCase("prd","manage-apprenticeships.service")]
        public void Then_The_Url_Is_Built_Correctly_For_Each_Environment(string environment, string expectedUrlPart)
        {
            var urlBuilder = new UrlBuilder(environment);
            Assert.AreEqual($"https://recruit.{expectedUrlPart}.gov.uk/accounts/ABC123", urlBuilder.RecruitLink("RecruitHome", "ABC123"));
        }
        
        [TestCase("at","at")]
        [TestCase("test","test")]
        [TestCase("test2","test2")]
        [TestCase("pp", "pp")]
        [TestCase("Mo", "mo")]
        [TestCase("Demo", "demo")]
        [TestCase("prd","beta")]
        public void Then_The_Url_Is_Built_Correctly_For_Each_Environment_For_Login(string environment, string expectedUrlPart)
        {
            var urlBuilder = new UrlBuilder(environment);
            Assert.AreEqual($"https://{expectedUrlPart}-login.apprenticeships.education.gov.uk/TermsAndConditions", urlBuilder.UsersLink("TermsAndConditions"));
        }
        
    }
}