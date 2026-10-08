using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using SFA.DAS.Telemetry.NotFoundResponseProcessor;

namespace SFA.DAS.Telemetry.UnitTests.NotFoundResponseProcessor;

public class NotFoundAsSuccessResponseProcessorTests
{
    private ActivityListener _activityListener = null!;
    private ActivitySource _activitySource = null!;
    private HttpContextAccessor _httpContextAccessor = null!;
    private NotFoundAsSuccessResponseProcessor _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _activitySource = new ActivitySource("test-source");

        _activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "test-source",
            Sample = (
                ref ActivityCreationOptions<ActivityContext> _) =>
                ActivitySamplingResult.AllData
        };

        ActivitySource.AddActivityListener(_activityListener);

        _httpContextAccessor = new HttpContextAccessor();
        _sut = new NotFoundAsSuccessResponseProcessor(_httpContextAccessor);
    }

    [TearDown]
    public void TearDown()
    {
        _activityListener.Dispose();
        _activitySource.Dispose();
        _sut.Dispose();
    }

    [Test]
    public void OnEnd_ServerActivityWith404AndMatchedEndpoint_MarksActivityAsSuccess()
    {
        DefaultHttpContext httpContext = CreateHttpContext(
            statusCode: StatusCodes.Status404NotFound,
            hasMatchedEndpoint: true);

        _httpContextAccessor.HttpContext = httpContext;

        using Activity activity = CreateActivity(ActivityKind.Server);

        _sut.OnEnd(activity);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Unset));
            Assert.That(activity.GetTagItem("error.type"), Is.Null);
            Assert.That(
                activity.GetTagItem("otel.status_code"),
                Is.EqualTo("OK"));
        }
    }

    [Test]
    public void OnEnd_ServerActivityWith404AndNoMatchedEndpoint_DoesNotModifyActivity()
    {
        DefaultHttpContext httpContext = CreateHttpContext(
            statusCode: StatusCodes.Status404NotFound,
            hasMatchedEndpoint: false);

        _httpContextAccessor.HttpContext = httpContext;

        using Activity activity = CreateFailedActivity();

        _sut.OnEnd(activity);

        AssertActivityRemainsFailed(activity);
    }

    [Test]
    public void OnEnd_NonServerActivityWith404_DoesNotModifyActivity()
    {
        DefaultHttpContext httpContext = CreateHttpContext(
            statusCode: StatusCodes.Status404NotFound,
            hasMatchedEndpoint: true);

        _httpContextAccessor.HttpContext = httpContext;

        using Activity activity = CreateFailedActivity(ActivityKind.Client);

        _sut.OnEnd(activity);

        AssertActivityRemainsFailed(activity);
    }

    [Test]
    public void OnEnd_ServerActivityWithNon404StatusCode_DoesNotModifyActivity()
    {
        DefaultHttpContext httpContext = CreateHttpContext(
            statusCode: StatusCodes.Status500InternalServerError,
            hasMatchedEndpoint: true);

        _httpContextAccessor.HttpContext = httpContext;

        using Activity activity = CreateFailedActivity();

        _sut.OnEnd(activity);

        AssertActivityRemainsFailed(activity);
    }

    [Test]
    public void OnEnd_WhenHttpContextIsNotAvailable_DoesNotModifyActivity()
    {
        _httpContextAccessor.HttpContext = null;

        using Activity activity = CreateFailedActivity();

        _sut.OnEnd(activity);

        AssertActivityRemainsFailed(activity);
    }

    private static DefaultHttpContext CreateHttpContext(
        int statusCode,
        bool hasMatchedEndpoint)
    {
        DefaultHttpContext httpContext = new()
        {
            Response =
            {
                StatusCode = statusCode
            }
        };

        if (hasMatchedEndpoint)
        {
            httpContext.SetEndpoint(
                new Endpoint(
                    _ => Task.CompletedTask,
                    EndpointMetadataCollection.Empty,
                    "Test endpoint"));
        }

        return httpContext;
    }

    private Activity CreateActivity(ActivityKind kind)
    {
        return _activitySource.StartActivity(
            "test-activity",
            kind)!;
    }

    private Activity CreateFailedActivity(
        ActivityKind kind = ActivityKind.Server)
    {
        Activity activity = CreateActivity(kind);

        activity.SetStatus(ActivityStatusCode.Error, "Test failure");
        activity.SetTag("error.type", "TestException");
        activity.SetTag("otel.status_code", "ERROR");

        return activity;
    }

    private static void AssertActivityRemainsFailed(Activity activity)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
            Assert.That(
                activity.GetTagItem("error.type"),
                Is.EqualTo("TestException"));
            Assert.That(
                activity.GetTagItem("otel.status_code"),
                Is.EqualTo("ERROR"));
        }
    }
}