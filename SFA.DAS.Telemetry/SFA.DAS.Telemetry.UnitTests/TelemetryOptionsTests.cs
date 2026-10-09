using NUnit.Framework;

namespace SFA.DAS.Telemetry.UnitTests;

public class TelemetryOptionsTests
{
    [Test]
    public void NewInstance_PopulatesDefaultValues()
    {
        var sut = new TelemetryOptions();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.EnableNotFoundAsSuccessResponse, Is.False);
            Assert.That(sut.UriRedactionOptions.EnableUriRedaction, Is.False);
            Assert.That(sut.UriRedactionOptions.RedactionList, Is.Empty);
            Assert.That(sut.UriRedactionOptions.RedactionValue, Is.EqualTo("REDACTED"));
        }
    }
    [Test]
    public void NewInstance_WithParameters_LoadsValues()
    {
        var sut = new TelemetryOptions(true, false, "email, dateOfBirth");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.EnableNotFoundAsSuccessResponse, Is.True);
            Assert.That(sut.UriRedactionOptions.EnableUriRedaction, Is.False);
            Assert.That(sut.UriRedactionOptions.RedactionList, Contains.Item("email"));
            Assert.That(sut.UriRedactionOptions.RedactionList, Contains.Item("dateOfBirth"));
            Assert.That(sut.UriRedactionOptions.RedactionValue, Is.EqualTo("REDACTED"));
        }
    }
}
