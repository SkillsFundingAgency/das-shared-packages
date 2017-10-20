using System;
using SFA.DAS.Messaging.Helpers;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging.AzureStorageQueue
{
    public class QueueMessageSubscriberFactory : IMessageSubscriberFactory
    {
        private readonly string _connectionString;

        public QueueMessageSubscriberFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IMessageSubscriber<T> GetSubscriber<T>() where T : new()
        {
            var messageGroupName = MessageGroupHelper.GetMessageGroupName<T>();

            return new QueueMessageSubscriber<T>(_connectionString, messageGroupName);
        }
    }
}
