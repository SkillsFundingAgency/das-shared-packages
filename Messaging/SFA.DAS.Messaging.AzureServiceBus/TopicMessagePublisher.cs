using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using SFA.DAS.Messaging.Helpers;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class TopicMessagePublisher : IMessagePublisher, IDisposable
    {
        private readonly ConcurrentDictionary<string, TopicClient> _clients = new ConcurrentDictionary<string, TopicClient>();
        private readonly string _connectionString;
        private readonly ILog _logger;

        public TopicMessagePublisher(string connectionString, ILog logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task PublishAsync(object message)
        {
            _logger.Trace("Getting message group name from message");

            var messageGroupName = MessageGroupHelper.GetMessageGroupName(message);
            
            _logger.Debug($"Adding message to message group {messageGroupName}");
                
            var client = _clients.GetOrAdd(messageGroupName, p => TopicClient.CreateFromConnectionString(_connectionString, p));

            await client.SendAsync(new BrokeredMessage(message));

            _logger.Debug($"Message has been added to message group {messageGroupName}");
        }

        public void Dispose()
        {
            foreach (var client in _clients.Values)
            {
                if (client != null && !client.IsClosed)
                {
                    _logger.Debug($"Closing topic message publisher for '{client.Path}'");
                    client.Close();
                }
            }
        }
    }
}