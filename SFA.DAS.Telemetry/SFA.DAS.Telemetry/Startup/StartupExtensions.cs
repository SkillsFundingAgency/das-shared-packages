using System;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Telemetry.RedactionService;
using SFA.DAS.Telemetry.Telemetry;

namespace SFA.DAS.Telemetry.Startup;

public static class StartupExtensions
{
    public static IServiceCollection AddTelemetryUriRedaction(this IServiceCollection serviceCollection, string keysForRedaction)
    {
        serviceCollection.AddSingleton<ITelemetryInitializer, UriRedactionTelemetryInitializer>();
        serviceCollection.AddSingleton<IUriRedactionService>(_ => new UriRedactionService(UriRedactionOptionsFactory.Create(keysForRedaction)));
        return serviceCollection;
    }

    public static IServiceCollection AddTelemetryUriRedaction(this IServiceCollection serviceCollection, Func<UriRedactionOptions> options)
    {
        serviceCollection.AddSingleton<ITelemetryInitializer, UriRedactionTelemetryInitializer>();
        serviceCollection.AddSingleton<IUriRedactionService, UriRedactionService>(s => new UriRedactionService(options.Invoke()));
        return serviceCollection;
    }

    public static IServiceCollection AddTelemetryNotFoundAsSuccessfulResponse(this IServiceCollection services)
    {
        services.AddSingleton<ITelemetryInitializer, NotFoundAsSuccessfulResponseTelemetryInitializer>();
        return services;
    }
}
