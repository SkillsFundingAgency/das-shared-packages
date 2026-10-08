using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using SFA.DAS.Telemetry.NotFoundResponseProcessor;
using SFA.DAS.Telemetry.RedactionProcessor;

namespace SFA.DAS.Telemetry.Extensions;

[ExcludeFromCodeCoverage]
public static class TracerProviderBuilderExtensions
{
    public static TracerProviderBuilder AddUriRedaction(this TracerProviderBuilder builder, string keysForRedaction)
    {
        return builder.AddProcessor(new UriRedactionActivityProcessor(new UriRedactionOptions(keysForRedaction)));
    }

    public static TracerProviderBuilder AddUriRedaction(this TracerProviderBuilder builder, UriRedactionOptions options)
    {
        return builder.AddProcessor(new UriRedactionActivityProcessor(options));
    }

    public static TracerProviderBuilder AddNotFoundResponseAsSuccessProcessor(this TracerProviderBuilder builder)
    {
        return builder.AddProcessor((services) => new NotFoundAsSuccessResponseProcessor(services.GetRequiredService<IHttpContextAccessor>()));
    }
}
