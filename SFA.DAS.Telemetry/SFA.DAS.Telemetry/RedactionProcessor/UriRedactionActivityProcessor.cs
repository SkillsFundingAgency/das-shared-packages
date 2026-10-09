using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using OpenTelemetry;

namespace SFA.DAS.Telemetry.RedactionProcessor;

public class UriRedactionActivityProcessor : BaseProcessor<Activity>
{
    private readonly Regex _queryParameterRegex;
    private readonly string _redactionValue = "[REDACTED]";

    public UriRedactionActivityProcessor(UriRedactionOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.RedactionList.Count == 0)
        {
            throw new ArgumentException("RedactionList cannot be empty.", nameof(options));
        }

        string parameterNames = string.Join(
            "|",
            options.RedactionList
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .Select(Regex.Escape));

        if (string.IsNullOrWhiteSpace(parameterNames))
        {
            throw new ArgumentException(
                "RedactionList must contain at least one valid key.",
                nameof(options));
        }

        _redactionValue = $"[{options.RedactionValue}]";

        _queryParameterRegex = new Regex(
            $@"(?<separator>[?&])(?<key>{parameterNames})=(?<value>[^&#\s]*)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase,
            TimeSpan.FromSeconds(10));
    }

    public override void OnEnd(Activity activity)
    {
        activity.DisplayName = Redact(activity.DisplayName);

        // traverse all tags in the activity and redact any values that match the redaction list
        foreach (KeyValuePair<string, object?> tag in activity.TagObjects.ToArray())
        {
            if (tag.Value is string value)
            {
                string redactedValue = Redact(value);

                if (!string.Equals(
                        value,
                        redactedValue,
                        StringComparison.Ordinal))
                {
                    activity.SetTag(tag.Key, redactedValue);
                }
            }
        }
    }

    private string Redact(string value)
    {
        if (!value.Contains('?'))
        {
            return value;
        }

        return _queryParameterRegex.Replace(
            value,
            match =>
                $"{match.Groups["separator"].Value}" +
                $"{match.Groups["key"].Value}={_redactionValue}");
    }
}
