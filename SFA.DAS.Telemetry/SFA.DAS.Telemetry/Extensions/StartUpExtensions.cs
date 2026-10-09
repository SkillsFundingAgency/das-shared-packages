using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Telemetry.Extensions;

namespace SFA.DAS.Telemetry.Extensions;

public static class StartUpExtensions
{
    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, TelemetryOptions options)
    {
        services
            .AddHttpContextAccessor()
            .AddOpenTelemetry()
            .UseAzureMonitor(o => o.ConnectionString = options.ApplicationInsightsConnectionString)
            .WithTracing(builder =>
            {
                if (options.EnableNotFoundAsSuccessResponse)
                {
                    builder.AddNotFoundResponseAsSuccessProcessor();
                }

                if (options.UriRedactionOptions.EnableUriRedaction)
                {
                    builder.AddUriRedaction(options.UriRedactionOptions);
                }
            });

        return services;
    }
}
