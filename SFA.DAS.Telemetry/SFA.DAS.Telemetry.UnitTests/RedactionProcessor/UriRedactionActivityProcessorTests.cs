using System.Diagnostics;
using NUnit.Framework;
using SFA.DAS.Telemetry.RedactionProcessor;

namespace SFA.DAS.Telemetry.UnitTests.RedactionProcessor;

public class UriRedactionActivityProcessorTests
{
    private UriRedactionActivityProcessor _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new UriRedactionOptions
        {
            RedactionList = ["email", "dateOfBirth"]
        };
        _sut = new UriRedactionActivityProcessor(options);
    }

    [TearDown]
    public void TearDown()
    {
        _sut.Dispose();
    }

    [Test]
    public void OnInstanceCreation_AndIfOptionsNotGiven_ThrowsNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new UriRedactionActivityProcessor(null!));
    }
    [Test]
    public void OnInstanceCreation_AndRedactionListIsEmpty_ThrowsArgumentException()
    {
        var options = new UriRedactionOptions
        {
            RedactionList = []
        };
        Assert.Throws<ArgumentException>(() => new UriRedactionActivityProcessor(options));
    }

    [TestCase("dependency", "http.url", "https://example.com/path?dateOfBirth=2019-11-10", "https://example.com/path?dateOfBirth=[REDACTED]")]
    [TestCase("request", "url.full", "https://example.com/path?email=test@example.com&keep=true", "https://example.com/path?email=[REDACTED]&keep=true")]
    [TestCase("unknown", "unknown", "?unkeyed&Email=test@example.com", "?unkeyed&Email=[REDACTED]")]
    public void IrrespectivelyAnyActivityAndAnyTagIsRedacted(string activityType, string tag, string actualValue, string expectedValue)
    {
        using var activity = new Activity(activityType);
        activity.SetTag(tag, actualValue);

        _sut.OnEnd(activity);

        Assert.That(activity.GetTagItem(tag), Is.EqualTo(expectedValue));
    }

    [Test]
    public void OnEnd_RedactsQueryValuesFromDisplayNameAndAllStringTags()
    {
        const string requestUrl =
            "https://example.com/search?email=test@example.com&keep=true";

        using var activity = new Activity(
            "GET /search?email=test@example.com&keep=true");

        activity.SetTag("http.url", requestUrl);
        activity.SetTag("trace.message", requestUrl);
        activity.SetTag("unrelated", "no query string");

        _sut.OnEnd(activity);

        const string expectedUrl =
            "https://example.com/search?email=[REDACTED]&keep=true";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                activity.DisplayName,
                Is.EqualTo("GET /search?email=[REDACTED]&keep=true"));

            Assert.That(
                activity.GetTagItem("http.url"),
                Is.EqualTo(expectedUrl));

            Assert.That(
                activity.GetTagItem("trace.message"),
                Is.EqualTo(expectedUrl));

            Assert.That(
                activity.GetTagItem("unrelated"),
                Is.EqualTo("no query string"));
        }
    }
}
