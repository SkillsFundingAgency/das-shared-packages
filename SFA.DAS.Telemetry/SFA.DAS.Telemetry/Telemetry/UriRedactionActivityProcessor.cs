using System;
using System.Diagnostics;
using OpenTelemetry;
using SFA.DAS.Telemetry.RedactionService;

namespace SFA.DAS.Telemetry.Telemetry;

public class UriRedactionActivityProcessor : BaseProcessor<Activity>
{
    private readonly IUriRedactionService _uriRedactionService;

    public UriRedactionActivityProcessor(IUriRedactionService uriRedactionService)
    {
        _uriRedactionService = uriRedactionService;
    }

    public override void OnEnd(Activity activity)
    {
        RedactAbsoluteUriTag(activity, "url.full");
        RedactAbsoluteUriTag(activity, "http.url");
        RedactRelativeUriTag(activity, "http.target");
        RedactUrlQueryTag(activity);
    }

    private void RedactAbsoluteUriTag(Activity activity, string tagName)
    {
        if (activity.GetTagItem(tagName) is string uriValue &&
            Uri.TryCreate(uriValue, UriKind.Absolute, out var uri))
        {
            activity.SetTag(tagName, _uriRedactionService.GetRedactedUri(uri).ToString());
        }
    }

    private void RedactRelativeUriTag(Activity activity, string tagName)
    {
        var target = activity.GetTagItem(tagName) as string;
        if (target == null)
        {
            return;
        }

        var scheme = activity.GetTagItem("url.scheme") as string ?? activity.GetTagItem("http.scheme") as string;
        var host = activity.GetTagItem("server.address") as string ?? activity.GetTagItem("http.host") as string;

        if (string.IsNullOrWhiteSpace(scheme) || string.IsNullOrWhiteSpace(host))
        {
            return;
        }

        if (!Uri.TryCreate($"{scheme}://{host}{target}", UriKind.Absolute, out var uri))
        {
            return;
        }

        var redactedUri = _uriRedactionService.GetRedactedUri(uri);
        activity.SetTag(tagName, $"{redactedUri.PathAndQuery}{redactedUri.Fragment}");
        activity.SetTag("url.query", redactedUri.Query);
    }

    private void RedactUrlQueryTag(Activity activity)
    {
        var query = activity.GetTagItem("url.query") as string;
        if (string.IsNullOrWhiteSpace(query))
        {
            return;
        }

        var scheme = activity.GetTagItem("url.scheme") as string ?? activity.GetTagItem("http.scheme") as string;
        var host = activity.GetTagItem("server.address") as string ?? activity.GetTagItem("http.host") as string;
        var path = activity.GetTagItem("url.path") as string;

        if (string.IsNullOrWhiteSpace(scheme) || string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        if (!Uri.TryCreate($"{scheme}://{host}{path}{query}", UriKind.Absolute, out var uri))
        {
            return;
        }

        var redactedUri = _uriRedactionService.GetRedactedUri(uri);

        activity.SetTag("url.query", redactedUri.Query);
        activity.SetTag("url.path", redactedUri.AbsolutePath);
        activity.SetTag("url.full", redactedUri.ToString());
    }
}
