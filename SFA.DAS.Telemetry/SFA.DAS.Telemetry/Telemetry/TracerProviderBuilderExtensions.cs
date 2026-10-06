using OpenTelemetry.Trace;
using SFA.DAS.Telemetry.RedactionService;

namespace SFA.DAS.Telemetry.Telemetry;

public static class TracerProviderBuilderExtensions
{
    public static TracerProviderBuilder AddUriRedaction(this TracerProviderBuilder builder, string keysForRedaction)
    {
        var redactionService = new UriRedactionService(UriRedactionOptionsFactory.Create(keysForRedaction));
        return builder.AddProcessor(new UriRedactionActivityProcessor(redactionService));
    }

    public static TracerProviderBuilder AddUriRedaction(this TracerProviderBuilder builder, UriRedactionOptions options)
    {
        return builder.AddProcessor(new UriRedactionActivityProcessor(new UriRedactionService(options)));
    }
}
