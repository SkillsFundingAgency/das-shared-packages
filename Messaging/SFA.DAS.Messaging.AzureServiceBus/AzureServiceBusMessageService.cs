using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class AzureServiceBusMessageService : IMessagePublisher, IPollingMessageReceiver
    {
        private readonly string _connectionString;
        private readonly string _queueName;


        public AzureServiceBusMessageService(string connectionString, string queueName = "")
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        public async Task PublishAsync(object message)
        {
            var queueNameAttributeValue = message.GetType()
                                            .CustomAttributes.FirstOrDefault(c => c.AttributeType == typeof(QueueNameAttribute))
                                            ?.ConstructorArguments.FirstOrDefault().Value;
            var queueName = message.GetType().Name;
            if (queueNameAttributeValue != null)
            {
                queueName = queueNameAttributeValue.ToString();
            }

            var client = QueueClient.CreateFromConnectionString(_connectionString, !string.IsNullOrEmpty(_queueName) ? _queueName : queueName);
            var brokeredMessage = new BrokeredMessage(message)
            {
                MessageId = Guid.NewGuid().ToString()
            };

            await client.SendAsync(brokeredMessage);
        }

        public async Task<IMessage<T>> ReceiveAsAsync<T>() where T : new()
        {
            var queueName = GetQueueName<T>();

            var client = QueueClient.CreateFromConnectionString(_connectionString, !string.IsNullOrEmpty(_queueName) ? _queueName : queueName);
            var brokeredMessage = await client.ReceiveAsync();
            if (brokeredMessage == null)
            {
                return null;
            }

            return new AzureServiceBusMessage<T>(brokeredMessage);
        }

        public async Task<IEnumerable<IMessage<T>>> ReceiveBatchAsAsync<T>(int batchSize) where T : new()
        {
            var queueName = GetQueueName<T>();

            var client = QueueClient.CreateFromConnectionString(_connectionString, !string.IsNullOrEmpty(_queueName) ? _queueName : queueName);
            var batch = await client.ReceiveBatchAsync(batchSize);
            return batch.Select(m => new AzureServiceBusMessage<T>(m));
        }

        private static string GetQueueName<T>() where T : new()
        {
            var queueNameAttributeValue =
                typeof(T).CustomAttributes.FirstOrDefault(c => c.AttributeType == typeof(QueueNameAttribute))
                    ?.ConstructorArguments.FirstOrDefault().Value;

            var queueName = typeof(T).Name;
            if (queueNameAttributeValue != null)
            {
                queueName = queueNameAttributeValue.ToString();
            }
            return queueName;
        }
    }
}
