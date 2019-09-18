using System;
using HMRC.ESFA.Levy.Api.Types.Exceptions;
using Polly;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Hmrc.ExecutionPolicy
{
    [PolicyName(Name)]
    public class HmrcExecutionPolicy : ExecutionPolicy
    {
        public const string Name = "HMRC Policy";

        private readonly ILog _logger;

        public HmrcExecutionPolicy(ILog logger) : this(logger, new TimeSpan(0, 0, 10))
        {
        }

        public HmrcExecutionPolicy(ILog logger, TimeSpan retryWaitTime)
        {
            _logger = logger;

            Policy tooManyRequestsPolicy = Policy.Handle<ApiHttpException>(ex => ex.HttpCode.Equals(429)).WaitAndRetryForeverAsync(i => retryWaitTime, (ex, ts) => OnRetryableFailure(ex));
            Policy requestTimeoutPolicy = Policy.Handle<ApiHttpException>(ex => ex.HttpCode.Equals(408)).WaitAndRetryForeverAsync(i => retryWaitTime, (ex, ts) => OnRetryableFailure(ex));
            var serviceUnavailablePolicy = CreateAsyncRetryPolicy<ApiHttpException>(ex => ex.HttpCode.Equals(503), 5, retryWaitTime, OnRetryableFailure);
            var internalServerErrorPolicy = CreateAsyncRetryPolicy<ApiHttpException>(ex => ex.HttpCode.Equals(500), 5, retryWaitTime, OnRetryableFailure);

            RootPolicy = Policy.WrapAsync(tooManyRequestsPolicy, serviceUnavailablePolicy, internalServerErrorPolicy, requestTimeoutPolicy);
        }

        protected override T OnException<T>(Exception ex)
        {
            if (ex is ApiHttpException exception)
            {
                _logger.Info($"ApiHttpException - {ex.Message}");

                switch (exception.HttpCode)
                {
                    case 404:
                        _logger.Info($"Resource not found - {ex.Message}");
                        return default;
                }
            }

            _logger.Error(ex, $"Exceeded retry limit - {ex.Message}");
            throw ex;
        }

        private void OnRetryableFailure(Exception ex)
        {
            _logger.Info($"Error calling HMRC - {ex.Message} - Will retry");
        }
    }
}