using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.ExecutionPolicies;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;


namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class TopicMessageSubscriber<T> : IMessageSubscriber<T> where T : new()
    {
        private readonly string _connectionString;
        private readonly string _topicName;
        private readonly string _subscriptionName;
        private readonly ExecutionPolicy _executionPolicy;
        private readonly ILog _logger;
        private readonly bool _keepConnectionAlive;
        private SubscriptionClient _client;
        private bool _clientOpen;

        public TopicMessageSubscriber(string connectionString, string topicName, string subscriptionName,
            [RequiredPolicy(PollyPolicyNames.TopicMessageSubscriberPolicyName)] ExecutionPolicy executionPolicy, 
            ILog logger, bool keepConnectionAlive = false)
        {
            _connectionString = connectionString;
            _topicName = topicName;
            _subscriptionName = subscriptionName;
            _executionPolicy = executionPolicy;
            _logger = logger;
            _keepConnectionAlive = keepConnectionAlive;
            _clientOpen = false;
        }

        public async Task<IMessage<T>> ReceiveAsAsync()
        {
            while (true)
            {
                return await _executionPolicy.ExecuteAsync(async () =>
                {
                    try
                    {
                        if (!_clientOpen)
                            OpenClient();

                        BrokeredMessage brokeredMessage;
                        IMessage<T> message;

                        do
                        {
                            brokeredMessage = await _client.ReceiveAsync();
                            message = await DeserialiseAzureBrokeredMessage(brokeredMessage);

                        } while (brokeredMessage != null && message == null);

                        return message;
                    }
                    catch (SessionLockLostException)
                    {
                        RefreshClient();
                        throw;
                    }
                });
            }
        }

        public async Task<IEnumerable<IMessage<T>>> ReceiveBatchAsAsync(int batchSize)
        {
            return await _executionPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    if (!_clientOpen)
                        OpenClient();
                   
                    var messages = new List<IMessage<T>>();

                    var brokeredMessages = await _client.ReceiveBatchAsync(batchSize);

                    foreach (var brokeredMessage in brokeredMessages)
                    {
                        var message = await DeserialiseAzureBrokeredMessage(brokeredMessage);

                        if(message != null)
                            messages.Add(message);
                    }

                    return messages;
                }
                catch (SessionLockLostException)
                {
                    RefreshClient();
                    throw;
                }
            });
        }

        public void Dispose()
        {
            CloseClient();
        }

        private void RefreshClient()
        {
            CloseClient();
            OpenClient();
        }

        private void OpenClient()
        {
            _client = SubscriptionClient.CreateFromConnectionString(_connectionString, _topicName, _subscriptionName);
            _clientOpen = true;
        }

        private void CloseClient()
        {
            if (_client != null && !_client.IsClosed)
            {
                _client.Close();
            }
            _clientOpen = false;
        }

        private async Task<IMessage<T>> DeserialiseAzureBrokeredMessage(BrokeredMessage brokeredMessage)
        {
            if (brokeredMessage == null) return null;

            try
            {
                return new AzureServiceBusMessage<T>(brokeredMessage, _logger, _keepConnectionAlive);
            }
            catch (Exception ex)
            {
                 _logger.Error(ex, "Could not deserialise message from azure queue. Message will be dead lettered.");

                //Dead letter the message as it can never be processed.
                await brokeredMessage.DeadLetterAsync("Message deserialisation failed", ex.Message);
            }

            return null;
        }
    }
}
