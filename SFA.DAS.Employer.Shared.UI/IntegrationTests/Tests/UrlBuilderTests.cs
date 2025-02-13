using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests.Tests;

public class UrlBuilderTests
{
    [Test]
    public void AccountsLinkWithNullAccountId_ReturnRouteLink()
    {
        var urlBuilder = new UrlBuilder("test");

        urlBuilder.AccountsLink().Should().Be("https://accounts.test-eas.apprenticeships.education.gov.uk/");
        urlBuilder.AccountsLink("AccountsHome", "").Should().Be("https://accounts.test-eas.apprenticeships.education.gov.uk/");
        urlBuilder.AccountsLink("AccountsHome", "ABC123").Should().Be("https://accounts.test-eas.apprenticeships.education.gov.uk/accounts/ABC123/teams");
        urlBuilder.AccountsLink("AccountsTeamsView", "ABC123").Should().Be("https://accounts.test-eas.apprenticeships.education.gov.uk/accounts/ABC123/teams/view");
    }

    [Test]
    public void Then_The_Commitments_Links_Are_Built()
    {
        var urlBuilder = new UrlBuilder("test");

        urlBuilder.CommitmentsV2Link("ApprenticesHome", "ABC123").Should().Be("https://approvals.test-eas.apprenticeships.education.gov.uk/ABC123");
        urlBuilder.CommitmentsV2Link("ApprenticeDetails", "ABC123", "ZZZ999").Should().Be("https://approvals.test-eas.apprenticeships.education.gov.uk/ABC123/apprentices/ZZZ999/details");
    }

    [Test]
    public void Then_The_Finance_Links_Are_Built()
    {
        var urlBuilder = new UrlBuilder("test");

        urlBuilder.FinanceLink("AccountsFinance", "ABC123").Should().Be("https://finance.test-eas.apprenticeships.education.gov.uk/accounts/ABC123/finance");
    }

    [Test]
    public void Then_The_Recruit_Links_Are_Built()
    {
        var urlBuilder = new UrlBuilder("test");

        urlBuilder
            .RecruitLink("RecruitHome", "ABC123")
            .Should().Be("https://recruit.test-eas.apprenticeships.education.gov.uk/accounts/ABC123");
    }

    [Test]
    public void Then_The_Employer_Profiles_Links_Are_Built()
    {
        var urlBuilder = new UrlBuilder("pp");

        urlBuilder
            .EmployerProfiles("ChangeLoginDetails", "ABC123")
            .Should().Be("https://employerprofiles.pp-eas.apprenticeships.education.gov.uk/accounts/ABC123/user/change-sign-in-details");
    }

    [Test]
    public void Then_The_User_Links_Are_Built()
    {
        var urlBuilder = new UrlBuilder("test");

        urlBuilder
            .UsersLink("ChangePassword", "ABC123", "return-address")
            .Should().Be("https://test-login.apprenticeships.education.gov.uk/account/changepassword?clientId=ABC123&returnUrl=return-address");
    }

    [Test]
    public void Then_The_Apprenticeships_Links_Are_Built()
    {
        var urlBuilder = new UrlBuilder("test");

        urlBuilder
            .ApprenticeshipsLink("ViewPendingPriceChange", "ABC123", "ZZZ999")
            .Should().Be("https://apprenticeshipdetails.test-eas.apprenticeships.education.gov.uk/employer/ABC123/ChangeOfPrice/ZZZ999/pending");
    }

    [TestCase("at", "at-eas.apprenticeships.education")]
    [TestCase("test", "test-eas.apprenticeships.education")]
    [TestCase("test2", "test2-eas.apprenticeships.education")]
    [TestCase("pp", "pp-eas.apprenticeships.education")]
    [TestCase("Mo", "mo-eas.apprenticeships.education")]
    [TestCase("Demo", "demo-eas.apprenticeships.education")]
    [TestCase("prd", "manage-apprenticeships.service")]
    public void Then_The_Url_Is_Built_Correctly_For_Each_Environment(string environment, string expectedUrlPart)
    {
        var urlBuilder = new UrlBuilder(environment);
        
        urlBuilder
            .RecruitLink("RecruitHome", "ABC123")
            .Should().Be($"https://recruit.{expectedUrlPart}.gov.uk/accounts/ABC123");
    }

    [TestCase("at", "at")]
    [TestCase("test", "test")]
    [TestCase("test2", "test2")]
    [TestCase("pp", "pp")]
    [TestCase("Mo", "mo")]
    [TestCase("Demo", "demo")]
    [TestCase("prd", "beta")]
    public void Then_The_Url_Is_Built_Correctly_For_Each_Environment_For_Login(string environment, string expectedUrlPart)
    {
        var urlBuilder = new UrlBuilder(environment);
        
        urlBuilder
            .UsersLink("TermsAndConditions")
            .Should().Be($"https://{expectedUrlPart}-login.apprenticeships.education.gov.uk/TermsAndConditions");
    }
}