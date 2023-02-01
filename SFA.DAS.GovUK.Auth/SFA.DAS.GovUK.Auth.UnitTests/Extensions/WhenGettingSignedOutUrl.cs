using SFA.DAS.GovUK.Auth.Extensions;

namespace SFA.DAS.GovUK.Auth.UnitTests.Extensions;

public class WhenGettingSignedOutUrl
{
    [TestCase("at","at-eas.apprenticeships.education")]
    [TestCase("test","test-eas.apprenticeships.education")]
    [TestCase("test2","test2-eas.apprenticeships.education")]
    [TestCase("pp", "pp-eas.apprenticeships.education")]
    [TestCase("Mo", "mo-eas.apprenticeships.education")]
    [TestCase("Demo", "demo-eas.apprenticeships.education")]
    [TestCase("prd","manage-apprenticeships.service")]
    public void Then_The_Url_Is_Built_Correctly_For_Each_Environment(string environment, string expectedUrlPart)
    {
        var actual = "".GetSignedOutRedirectUrl(environment);
        
        Assert.That(actual, Is.EqualTo($"https://employerprofiles.{expectedUrlPart}.gov.uk/service/user-signed-out"));
    }

    [Test]
    public void Then_If_The_Url_Is_Not_Empty_Then_Value_Returned()
    {
        var actual = "something".GetSignedOutRedirectUrl("environment");
        
        Assert.That(actual, Is.EqualTo("something"));
    }
}