using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using OpenTelemetry;

namespace SFA.DAS.Telemetry.RedactionProcessor;

public class UriRedactionActivityProcessor : BaseProcessor<Activity>
{
    private readonly Regex _redactionRegex;

    public UriRedactionActivityProcessor(UriRedactionOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.RedactionList.Count == 0)
        {
            throw new ArgumentException("RedactionList cannot be empty.", nameof(options));
        }

        _redactionRegex = new Regex(
            $@"(?i)\b({string.Join("|", options.RedactionList)})=([^&\s]+)",
            RegexOptions.Compiled,
            TimeSpan.FromSeconds(10));
    }

    public override void OnEnd(Activity activity)
    {
        // traverse all tags in the activity and redact any values that match the redaction list
        foreach (var tag in activity.TagObjects)
        {
            if (tag.Value is string tagValue && tagValue.Contains('?'))
            {
                string redactedValue = _redactionRegex.Replace(tagValue, "$1=[REDACTED]");
                activity.SetTag(tag.Key, redactedValue);
            }
        }
    }


}
