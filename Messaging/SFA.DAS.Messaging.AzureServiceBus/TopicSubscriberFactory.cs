using SFA.DAS.Messaging.Helper;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class TopicSubscriberFactory : IMessageSubscriberFactory 
    {
        private readonly string _connectionString;
        private readonly string _subscriptionName;

        public TopicSubscriberFactory(string connectionString, string subscriptionName)
        {
            _connectionString = connectionString;
            _subscriptionName = subscriptionName;
        }

        public IMessageSubscriber<T> GetSubscriber<T>() where T : new()
        {
            var messageGroupName = MessageGroupHelper.GetMessageGroupName<T>();

            return new TopicMessageSubscriber<T>(_connectionString, messageGroupName, _subscriptionName);
        }
    }
}
