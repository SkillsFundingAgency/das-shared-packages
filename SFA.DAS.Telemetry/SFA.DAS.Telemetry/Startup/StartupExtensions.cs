using System;
using System.Linq;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Telemetry.RedactionService;
using SFA.DAS.Telemetry.Telemetry;

namespace SFA.DAS.Telemetry.Startup
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddTelemetryUriRedaction(this IServiceCollection serviceCollection, string keysForRedaction)
        {
            serviceCollection.AddSingleton<ITelemetryInitializer, UriRedactionTelemetryInitializer>();

            var options = new UriRedactionOptions
            {
                RedactionList = keysForRedaction.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList()
            };

            serviceCollection.AddSingleton<IUriRedactionService, UriRedactionService>(s => new UriRedactionService(options));
            return serviceCollection;
        }

        public static IServiceCollection AddTelemetryUriRedaction(this IServiceCollection serviceCollection, Func<UriRedactionOptions> options)
        {
            serviceCollection.AddSingleton<ITelemetryInitializer, UriRedactionTelemetryInitializer>();
            serviceCollection.AddSingleton<IUriRedactionService, UriRedactionService>(s => new UriRedactionService(options.Invoke()));
            return serviceCollection;
        }
    }
}
