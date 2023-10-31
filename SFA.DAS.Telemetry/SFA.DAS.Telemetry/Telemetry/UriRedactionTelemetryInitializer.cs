using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using SFA.DAS.Telemetry.RedactionService;

namespace SFA.DAS.Telemetry.Telemetry
{
    public class UriRedactionTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IUriRedactionService _uriRedactionService;

        public UriRedactionTelemetryInitializer(IUriRedactionService uriRedactionService)
        {
            _uriRedactionService = uriRedactionService;
        }

        public void Initialize(ITelemetry telemetry)
        {
            switch (telemetry)
            {
                case RequestTelemetry requestTelemetry:
                    requestTelemetry.Url = _uriRedactionService.GetRedactedUri(requestTelemetry.Url);
                    break;
                case DependencyTelemetry dependencyTelemetry:
                    {
                        if (Uri.TryCreate(dependencyTelemetry.Data, UriKind.Absolute, out var dependencyUrl))
                        {
                            dependencyTelemetry.Data = _uriRedactionService.GetRedactedUri(dependencyUrl).ToString();
                        }

                        break;
                    }
            }
        }
    }
}
