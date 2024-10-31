using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace SFA.DAS.Telemetry.Telemetry
{
    public class NotFoundAsSuccessfulResponseTelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            switch (telemetry)
            {
                case RequestTelemetry request
                when request.ResponseCode == "404":
                    request.Success = true;
                    break;
            }
        }
    }
}
