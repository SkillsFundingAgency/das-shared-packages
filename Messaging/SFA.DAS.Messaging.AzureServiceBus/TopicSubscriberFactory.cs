using SFA.DAS.Messaging.AzureServiceBus.ExecutionPolicies;
using SFA.DAS.Messaging.Helpers;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class TopicSubscriberFactory : IMessageSubscriberFactory 
    {
        private readonly string _connectionString;
        private readonly string _subscriptionName;
        private readonly ILog _logger;
        private readonly bool _keepConnectionAlive;
        private readonly ExecutionPolicy _executionPolicy;

        public TopicSubscriberFactory(string connectionString, string subscriptionName, ILog logger, bool keepConnectionAlive = false)
        {
            _connectionString = connectionString;
            _subscriptionName = subscriptionName;
            _logger = logger;
            _keepConnectionAlive = keepConnectionAlive;
            _executionPolicy = new TopicSubscriberDefaultPolicy(logger);
        }

        public TopicSubscriberFactory(string connectionString, string subscriptionName, ILog logger, 
            [RequiredPolicy(PollyPolicyNames.TopicMessageSubscriberPolicyName)] ExecutionPolicy executionPolicy)
        {
            _connectionString = connectionString;
            _subscriptionName = subscriptionName;
            _logger = logger;
            _executionPolicy = executionPolicy;
        }

        public IMessageSubscriber<T> GetSubscriber<T>() where T : new()
        {
            _logger.Debug($"Getting subscriber of message type {typeof(T).FullName} from factory");

            var messageGroupName = MessageGroupHelper.GetMessageGroupName<T>();

            _logger.Debug($"Obtained message group name {messageGroupName} for subscriber of message type {typeof(T).FullName}");

            return new TopicMessageSubscriber<T>(_connectionString, messageGroupName, _subscriptionName, _executionPolicy, _logger, _keepConnectionAlive);
        }
    }
}
