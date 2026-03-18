using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Apim.Shared.InnerApi.InnerApi.Requests;
using SFA.DAS.Apim.Shared.Interfaces;

namespace SFA.DAS.Apim.Shared.Infrastructure.HealthCheck
{
    public class ApiHealthCheck<T>
    {
        private readonly string _healthCheckDescription;
        private readonly string _healthCheckResultDescription;

        private readonly IGetApiClient<T> _apiClient;
        private readonly ILogger<ApiHealthCheck<T>> _logger;

        public ApiHealthCheck(string healthCheckDescription, string healthCheckResultDescription,
            IGetApiClient<T> apiClient, ILogger<ApiHealthCheck<T>> logger)
        {
            _healthCheckDescription = healthCheckDescription;
            _healthCheckResultDescription = healthCheckResultDescription;
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
        {
            _logger.LogInformation($"Pinging {_healthCheckDescription}");

            var timer = Stopwatch.StartNew();
            var response = await _apiClient.GetResponseCode(new GetPingRequest());
            timer.Stop();

            if (response == HttpStatusCode.OK)
            {
                var durationString = timer.Elapsed.ToHumanReadableString();

                _logger.LogInformation($"{_healthCheckDescription} ping successful and took {durationString}");

                return HealthCheckResult.Healthy(_healthCheckResultDescription,
                    new Dictionary<string, object> { { "Duration", durationString } });
            }

            _logger.LogWarning($"{_healthCheckDescription} ping failed : [Code: {response}]");
            return HealthCheckResult.Unhealthy(_healthCheckResultDescription);
        }
    }
}
