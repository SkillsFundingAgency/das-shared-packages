using System;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Telemetry.Extensions;

namespace SFA.DAS.Telemetry.Extensions;

public static class StartUpExtensions
{
    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, Func<TelemetryOptions> telemetryOptions)
    {
        TelemetryOptions options = telemetryOptions();
        services
            .AddHttpContextAccessor()
            .AddOpenTelemetry()
            .UseAzureMonitor()
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
