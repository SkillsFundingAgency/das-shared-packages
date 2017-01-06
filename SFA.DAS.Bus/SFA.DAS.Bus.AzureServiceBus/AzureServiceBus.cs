using System;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace SFA.DAS.Bus.AzureServiceBus
{
    public class AzureServiceBus
    {
        private readonly string _connectionString;
        private string _subscription = "AllMessages";

        private OnMessageOptions _onMessageOptions = new OnMessageOptions
        {
            AutoComplete = false,
            AutoRenewTimeout = TimeSpan.FromMinutes(1)
        };

        public AzureServiceBus(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void OnMessage<T>(Action<T> callback, ManualResetEvent completedEvent)
        {
            var topic = GetTopic<T>();
            CreateSubscriptionIfItDoesntExist(topic);

            SubscriptionClient client = SubscriptionClient.CreateFromConnectionString(_connectionString, topic, _subscription);
 
            client.OnMessage(receivedMessage =>
            {
                HandleMessage(callback, receivedMessage);
            }, _onMessageOptions);

            completedEvent.WaitOne();
        }

        private static void HandleMessage<T>(Action<T> callback, BrokeredMessage receivedMessage)
        {
            var messageData = receivedMessage.GetBody<T>();
            callback(messageData);
            receivedMessage.Complete();
        }

        private void CreateSubscriptionIfItDoesntExist(string topic)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_connectionString);

            if (!namespaceManager.SubscriptionExists(topic, _subscription))
            {
                namespaceManager.CreateSubscription(topic, _subscription);
            }
        }

        private string GetTopic<T>()
        {
            return typeof(T).FullName;
        }
    }
}
