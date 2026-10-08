using NUnit.Framework;
using SFA.DAS.Telemetry.RedactionProcessor;

namespace SFA.DAS.Telemetry.UnitTests.RedactionProcessor;

public class UriRedactionOptionsTests
{
    [Test]
    public void NewInstance_PopulatesDefaultValues()
    {
        var sut = new UriRedactionOptions();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.EnableUriRedaction, Is.False);
            Assert.That(sut.RedactionList, Is.Empty);
            Assert.That(sut.RedactionString, Is.EqualTo("REDACTED"));
        }
    }

    [Test]
    public void NewInstance_WithParameters_LoadsRedactionList()
    {
        var sut = new UriRedactionOptions
        {
            EnableUriRedaction = true,
            RedactionList = ["email", "dateOfBirth"],
            RedactionString = "[REDACTED]"
        };
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sut.EnableUriRedaction, Is.True);
            Assert.That(sut.RedactionList, Contains.Item("email"));
            Assert.That(sut.RedactionList, Contains.Item("dateOfBirth"));
            Assert.That(sut.RedactionString, Is.EqualTo("[REDACTED]"));
        }
    }
}
