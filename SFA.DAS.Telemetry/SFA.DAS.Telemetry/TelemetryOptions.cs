using SFA.DAS.Telemetry.RedactionProcessor;

namespace SFA.DAS.Telemetry;

public class TelemetryOptions
{
    public string ApplicationInsightsConnectionString { get; set; } = "";
    public bool EnableNotFoundAsSuccessResponse { get; set; }
    public UriRedactionOptions UriRedactionOptions { get; set; } = new();
    public TelemetryOptions() { }

    public TelemetryOptions(bool enableNotFoundAsSuccessResponse, bool enableUriRedaction, string? commaDelimitedRedactionList = null)
    {
        EnableNotFoundAsSuccessResponse = enableNotFoundAsSuccessResponse;
        UriRedactionOptions = new UriRedactionOptions(commaDelimitedRedactionList) { EnableUriRedaction = enableUriRedaction };
    }
}
