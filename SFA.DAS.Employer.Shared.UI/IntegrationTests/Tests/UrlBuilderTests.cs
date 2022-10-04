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
        public void Then_The_User_Links_Are_Built()
        {
            var urlBuilder = new UrlBuilder("test");
            
            Assert.AreEqual("https://test-login.apprenticeships.education.gov.uk/account/changepassword?clientId=ABC123&returnUrl=return-address", urlBuilder.UsersLink("ChangePassword", new [] {"ABC123", "return-address"})); 
        }
        
    }
}