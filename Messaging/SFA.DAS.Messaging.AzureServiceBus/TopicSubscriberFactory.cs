﻿using SFA.DAS.Messaging.Helpers;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class TopicSubscriberFactory : IMessageSubscriberFactory 
    {
        private readonly string _connectionString;
        private readonly string _subscriptionName;
        private readonly ILog _logger;

        public TopicSubscriberFactory(string connectionString, string subscriptionName, ILog logger)
        {
            _connectionString = connectionString;
            _subscriptionName = subscriptionName;
            _logger = logger;
        }

        public IMessageSubscriber<T> GetSubscriber<T>() where T : new()
        {
            _logger.Debug($"Getting subscriber of message type {typeof(T).FullName} from factory");

            var messageGroupName = MessageGroupHelper.GetMessageGroupName<T>();

            _logger.Debug($"Obtained message group name {messageGroupName} for subscriber of message type {typeof(T).FullName}");

            return new TopicMessageSubscriber<T>(_connectionString, messageGroupName, _subscriptionName, _logger);
        }
    }
}
