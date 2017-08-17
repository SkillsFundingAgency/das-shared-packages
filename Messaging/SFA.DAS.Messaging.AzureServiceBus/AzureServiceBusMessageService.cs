﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

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
            var client = QueueClient.CreateFromConnectionString(_connectionString, !string.IsNullOrEmpty(_queueName) ? _queueName : message.GetType().Name);
            var brokeredMessage = new BrokeredMessage(message)
            {
                MessageId = Guid.NewGuid().ToString()
            };

            await client.SendAsync(brokeredMessage);
        }

        public async Task<IMessage<T>> ReceiveAsAsync<T>() where T : new()
        {
            var client = QueueClient.CreateFromConnectionString(_connectionString, !string.IsNullOrEmpty(_queueName) ? _queueName : typeof(T).Name);
            var brokeredMessage = await client.ReceiveAsync();
            if (brokeredMessage == null)
            {
                return null;
            }

            return new AzureServiceBusMessage<T>(brokeredMessage);
        }
        public async Task<IEnumerable<IMessage<T>>> ReceiveBatchAsAsync<T>(int batchSize) where T : new()
        {
            var client = QueueClient.CreateFromConnectionString(_connectionString, !string.IsNullOrEmpty(_queueName) ? _queueName : typeof(T).Name);
            var batch = await client.ReceiveBatchAsync(batchSize);
            return batch.Select(m => new AzureServiceBusMessage<T>(m));
        }
        
    }
}
