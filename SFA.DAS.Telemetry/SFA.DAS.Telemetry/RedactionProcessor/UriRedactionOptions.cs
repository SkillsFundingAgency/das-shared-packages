using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Telemetry.RedactionProcessor;

public class UriRedactionOptions
{
    public bool EnableUriRedaction { get; set; }
    public List<string> RedactionList { get; set; } = [];
    public string RedactionValue { get; set; } = "REDACTED";
    public UriRedactionOptions() { }
    public UriRedactionOptions(string? commaDelimitedRedactionList)
    {
        if (string.IsNullOrEmpty(commaDelimitedRedactionList)) return;

        RedactionList = [.. commaDelimitedRedactionList.Split(',', System.StringSplitOptions.RemoveEmptyEntries).Select(k => k.Trim())];
    }
}
