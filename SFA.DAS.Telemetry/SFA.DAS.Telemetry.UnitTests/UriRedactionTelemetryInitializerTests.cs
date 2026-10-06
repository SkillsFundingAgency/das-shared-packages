using Microsoft.ApplicationInsights.DataContracts;
using Moq;
using NUnit.Framework;
using SFA.DAS.Telemetry.RedactionService;
using SFA.DAS.Telemetry.Telemetry;

namespace SFA.DAS.Telemetry.UnitTests;

public class UriRedactionTelemetryInitializerTests
{
    private UriRedactionTelemetryInitializer _telemetryInitializer;
    private Mock<IUriRedactionService> _redactionService;
    private Uri _redactedUri;

    [SetUp]
    public void SetUp()
    {
        _redactedUri = new Uri("http://www.redacted.com");

        _redactionService = new Mock<IUriRedactionService>();
        _redactionService.Setup(x => x.GetRedactedUri(It.IsAny<Uri>())).Returns(_redactedUri);
        _redactionService.Setup(x => x.GetRedactedString(It.IsAny<string>())).Returns(_redactedUri.ToString());

        _telemetryInitializer = new UriRedactionTelemetryInitializer(_redactionService.Object);
    }

    [Test]
    public void RequestTelemetryUriIsRedacted()
    {
        var telemetry = new RequestTelemetry
        {
            Url = new Uri("http://www.request.com")
        };

        _telemetryInitializer.Initialize(telemetry);

        Assert.That(telemetry.Url, Is.EqualTo(_redactedUri));
    }

    [Test]
    public void DependencyTelemetryUriIsRedacted()
    {
        var telemetry = new DependencyTelemetry
        {
            Data = new Uri("http://www.request.com").ToString()
        };

        _telemetryInitializer.Initialize(telemetry);

        Assert.That(telemetry.Data, Is.EqualTo(_redactedUri.ToString()));
    }

    [Test]
    public void TraceTelemetryUriIsRedacted()
    {
        var telemetry = new TraceTelemetry
        {
            Message = "Request started http://www.request.com?email=chris@private.com&isRobot=false"
        };

        _telemetryInitializer.Initialize(telemetry);

        Assert.That(telemetry.Message, Is.EqualTo(_redactedUri.ToString()));
    }

    [Test]
    public void TraceTelemetryPropertiesAreRedacted()
    {
        var telemetry = new TraceTelemetry
        {
            Properties = { { "OriginalFormat", "Request started http://www.request.com?email=chris@private.com&isRobot=false" } },
            Message = "Request started http://www.request.com?email=chris@private.com&isRobot=false"
        };

        _telemetryInitializer.Initialize(telemetry);

        Assert.That(telemetry.Properties["OriginalFormat"], Is.EqualTo(_redactedUri.ToString()));
    }
}
