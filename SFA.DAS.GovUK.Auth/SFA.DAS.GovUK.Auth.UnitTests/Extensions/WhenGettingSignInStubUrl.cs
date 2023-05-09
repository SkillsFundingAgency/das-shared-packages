using SFA.DAS.GovUK.Auth.Extensions;

namespace SFA.DAS.GovUK.Auth.UnitTests.Extensions;

public class WhenGettingSignInStubUrl
{
    [TestCase("PrD")]
    [TestCase("local")]
    public void Then_If_Production_Or_Local_Returns_Empty(string environment)
    {
        var actual = RedirectExtension.GetStubSignInRedirectUrl(environment);
        
        Assert.IsEmpty(actual);
    }
    
    [TestCase("at","at-eas.apprenticeships.education")]
    [TestCase("test","test-eas.apprenticeships.education")]
    [TestCase("test2","test2-eas.apprenticeships.education")]
    [TestCase("pp", "pp-eas.apprenticeships.education")]
    [TestCase("Mo", "mo-eas.apprenticeships.education")]
    [TestCase("Demo", "demo-eas.apprenticeships.education")]
    public void Then_The_Url_Is_Built_Correctly_For_Each_Environment(string environment, string expectedUrlPart)
    {
        var actual = RedirectExtension.GetStubSignInRedirectUrl(environment);
        
        Assert.That(actual, Is.EqualTo($"https://employerprofiles.{expectedUrlPart}.gov.uk/service/account-details"));
    }

}