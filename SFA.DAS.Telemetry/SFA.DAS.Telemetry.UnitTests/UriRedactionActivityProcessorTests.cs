using System.Diagnostics;
using NUnit.Framework;
using SFA.DAS.Telemetry.RedactionService;
using SFA.DAS.Telemetry.Telemetry;

namespace SFA.DAS.Telemetry.UnitTests;

public class UriRedactionActivityProcessorTests
{
    private UriRedactionActivityProcessor _processor = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new UriRedactionOptions
        {
            RedactionList = new List<string> { "email", "dateOfBirth" }
        };

        _processor = new UriRedactionActivityProcessor(new UriRedactionService(options));
    }

    [Test]
    public void UrlFullTagIsRedacted()
    {
        using var activity = new Activity("request");
        activity.SetTag("url.full", "https://example.com/path?email=test@example.com&keep=true");

        _processor.OnEnd(activity);

        Assert.That(activity.GetTagItem("url.full"), Is.EqualTo("https://example.com/path?email=REDACTED&keep=true"));
    }

    [Test]
    public void HttpUrlTagIsRedacted()
    {
        using var activity = new Activity("dependency");
        activity.SetTag("http.url", "https://example.com/path?dateOfBirth=2019-11-10");

        _processor.OnEnd(activity);

        Assert.That(activity.GetTagItem("http.url"), Is.EqualTo("https://example.com/path?dateOfBirth=REDACTED"));
    }

    [Test]
    public void HttpTargetAndUrlQueryTagsAreRedacted()
    {
        using var activity = new Activity("request");
        activity.SetTag("http.target", "/path?Email=test@example.com&keep=true");
        activity.SetTag("url.scheme", "https");
        activity.SetTag("server.address", "example.com");

        _processor.OnEnd(activity);

        Assert.That(activity.GetTagItem("http.target"), Is.EqualTo("/path?Email=REDACTED&keep=true"));
        Assert.That(activity.GetTagItem("url.query"), Is.EqualTo("?Email=REDACTED&keep=true"));
    }

    [Test]
    public void HttpTargetIsLeftAloneWhenSchemeOrHostMissing()
    {
        using var activity = new Activity("request");
        activity.SetTag("http.target", "/path?email=test@example.com");

        _processor.OnEnd(activity);

        Assert.That(activity.GetTagItem("http.target"), Is.EqualTo("/path?email=test@example.com"));
    }
}
