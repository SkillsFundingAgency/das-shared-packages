using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class TopicSubscriberFactory<T> : IMessageSubscriberFactory<T> where T : new()
    {
        private readonly string _connectionString;
        private readonly string _topicName;
        private readonly string _subscriptionName;

        public TopicSubscriberFactory(string connectionString, string topicName, string subscriptionName)
        {
            _connectionString = connectionString;
            _topicName = topicName;
            _subscriptionName = subscriptionName;
        }

        public IMessageSubscriber<T> GetSubscriber()
        {
            return new TopicMessageSubscriber<T>(_connectionString, _topicName, _subscriptionName);
        }
    }
}
