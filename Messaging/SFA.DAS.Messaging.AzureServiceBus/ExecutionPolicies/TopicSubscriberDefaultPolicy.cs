using System;
using Microsoft.ServiceBus.Messaging;
using Polly;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Messaging.AzureServiceBus.ExecutionPolicies
{
    [PolicyName(PollyPolicyNames.TopicMessageSubscriberPolicyName)]
    public class TopicSubscriberDefaultPolicy : ExecutionPolicy
    {
        private readonly ILog _logger;
        private readonly Policy TimeoutExceptionPolicy;
        private readonly Policy UnauthorizedAccessExceptionPolicy;
        private readonly Policy ArgumentExceptionPolicy;
        private readonly Policy MessagingEntityNotFoundExceptionPolicy;
        private readonly Policy MessageNotFoundExceptionPolicy;
        private readonly Policy MessagingCommunicationExceptionPolicy;
        private readonly Policy ServerBusyExceptionPolicy;
        private readonly Policy SessionLockLostExceptionPolicy;
        private readonly Policy QuotaExceededExceptionPolicy;
        private readonly Policy MessagingExceptionPolicy;



        public TopicSubscriberDefaultPolicy(ILog logger)
        {
            _logger = logger;

            TimeoutExceptionPolicy =
                CreateAsyncRetryPolicy<TimeoutException>(4, new TimeSpan(0, 0, 1), OnRetryableFailure);
            UnauthorizedAccessExceptionPolicy =
                CreateAsyncRetryPolicy<UnauthorizedAccessException>(4, new TimeSpan(0, 0, 1), OnRetryableFailure);
            ArgumentExceptionPolicy =
                CreateAsyncRetryPolicy<ArgumentException>(4, new TimeSpan(0, 0, 1), OnRetryableFailure);
            MessagingEntityNotFoundExceptionPolicy =
                CreateAsyncRetryPolicy<MessagingEntityNotFoundException>(4, new TimeSpan(0, 0, 1), OnRetryableFailure);
            MessageNotFoundExceptionPolicy =
                CreateAsyncRetryPolicy<MessageNotFoundException>(4, new TimeSpan(0, 0, 1), OnRetryableFailure);
            MessagingCommunicationExceptionPolicy =
                CreateAsyncRetryPolicy<MessagingCommunicationException>(4, new TimeSpan(0, 0, 1), OnRetryableFailure);
            ServerBusyExceptionPolicy =
                CreateAsyncRetryPolicy<ServerBusyException>(4, new TimeSpan(0, 0, 1), OnRetryableFailure);
            SessionLockLostExceptionPolicy =
                CreateAsyncRetryPolicy<SessionLockLostException>(4, new TimeSpan(0, 0, 1), OnRetryableFailure);
            QuotaExceededExceptionPolicy =
                CreateAsyncRetryPolicy<QuotaExceededException>(4, new TimeSpan(0, 0, 1), OnRetryableFailure);
            MessagingExceptionPolicy =
                CreateAsyncRetryPolicy<MessagingException>(4, new TimeSpan(0, 0, 1), OnRetryableFailure);

            RootPolicy = Policy.WrapAsync(
                TimeoutExceptionPolicy, 
                UnauthorizedAccessExceptionPolicy,
                ArgumentExceptionPolicy,
                MessagingEntityNotFoundExceptionPolicy,
                MessageNotFoundExceptionPolicy,
                MessagingCommunicationExceptionPolicy,
                ServerBusyExceptionPolicy,
                SessionLockLostExceptionPolicy,
                QuotaExceededExceptionPolicy,
                MessagingExceptionPolicy);
        }

        protected override T OnException<T>(Exception ex)
        {
            _logger.Fatal(ex, "Unable to process any messages from azure service bus queue.");

            return default(T);
        }

        private void OnRetryableFailure(Exception ex)
        {
            _logger.Error(ex, "Could not get message from azure servive bus queue.");
        }
    }
}

