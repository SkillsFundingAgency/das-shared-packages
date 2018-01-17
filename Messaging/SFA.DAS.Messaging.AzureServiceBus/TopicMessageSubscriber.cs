using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;


namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class TopicMessageSubscriber<T> : IMessageSubscriber<T> where T: new()
    {
        private const int MessageRetrievalRetryLimit = 4;
        private const int MaxReconnectionLimit = 3;

        private readonly string _connectionString;
        private readonly string _topicName;
        private readonly string _subscriptionName;
        private readonly ILog _logger;
        private SubscriptionClient _client;
        private bool _clientOpen;

        public TopicMessageSubscriber(string connectionString, string topicName, string subscriptionName, ILog logger)
        {
            _connectionString = connectionString;
            _topicName = topicName;
            _subscriptionName = subscriptionName;
            _logger = logger;
            _clientOpen = false;
        }

        public async Task<IMessage<T>> ReceiveAsAsync()
        {
            if(!_clientOpen)
                OpenClient();

            return await GetNextMessage();
        }

        public async Task<IEnumerable<IMessage<T>>> ReceiveBatchAsAsync(int batchSize)
        {
            if (!_clientOpen)
                OpenClient();

            var brokerMessages = await _client.ReceiveBatchAsync(batchSize);

            return brokerMessages?.Select(msg => new AzureServiceBusMessage<T>(msg)).ToArray();
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

        private async Task<AzureServiceBusMessage<T>> GetNextMessage()
        {
            BrokeredMessage brokeredMessage = null;
            AzureServiceBusMessage<T> message = null;
            var requestHandled = false;
            var retryCount = 0;
            var reconnectionCount = 0;
            var serverBusyWaitMultipler = 1;

            while (!requestHandled && retryCount < MessageRetrievalRetryLimit)
            {
                try
                {
                    brokeredMessage = await _client.ReceiveAsync();
                    message = brokeredMessage == null ? null : new AzureServiceBusMessage<T>(brokeredMessage);
                    requestHandled = true;
                }
                catch (TimeoutException ex)
                {
                    _logger.Warn(ex, "Message could not be retrieved due to timeout.");
                    retryCount++;
                }
                catch (InvalidOperationException ex)
                {
                    _logger.Error(ex, "Message could not be retrieved due to invalid operation.");
                    throw;
                }
                catch (OperationCanceledException ex)
                {
                    _logger.Warn(ex, "Message has already be closed or disposed.");
                    break;
                }
                catch (UnauthorizedAccessException ex)
                {
                    _logger.Warn(ex, "Message could not be retrieved due to unauthorised access.");
                    retryCount++;
                }
                catch (ArgumentException ex)
                {
                    _logger.Error(ex, "Message could not be retrieved due to invalid argument.");
                    throw;
                }
                catch (MessagingEntityNotFoundException ex)
                {
                    _logger.Warn(ex, "Message entity not found.");
                    break;
                }
                catch (MessageNotFoundException ex)
                {
                    _logger.Warn(ex, "Message not found. It may have been disposed of already");
                    break;
                }
                catch (MessagingCommunicationException ex)
                {
                    _logger.Warn(ex, "Could not retrieve message due to communication issues.");
                    retryCount++;
                }
                catch (ServerBusyException ex)
                {
                    _logger.Warn(ex,
                        "Could not retrieve message due to server being busy. Waiting and then retrying...");
                    await Task.Delay(serverBusyWaitMultipler * 1000); //Wait a little longer if the server is still busy
                    serverBusyWaitMultipler++; //Make a note to wait a second longer if we get server busy again.
                    retryCount++;
                }
                catch (MessageLockLostException ex)
                {
                    _logger.Warn(ex,
                        "Could not retrieve message due to the lock expirying. Getting new message...");
                    retryCount++;
                }
                catch (SessionLockLostException ex)
                {
                    reconnectionCount++;

                    if (reconnectionCount < MaxReconnectionLimit)
                    {
                        _logger.Warn(ex, "Session lock lost. Restarting client");
                        RefreshClient();

                        retryCount = 0; //reset the retry count as its a new connection
                    }
                    else
                    {
                        _logger.Error(ex, "Maxed out reconnection retries to solve issue.");
                        throw;
                    }
                }
                catch (QuotaExceededException ex)
                {
                    _logger.Warn(ex, "Message queue quota exceeded. Will wait and retry.");
                    await Task.Delay(1000);
                    retryCount++;
                }
                catch (MessagingException ex)
                {
                    _logger.Warn(ex,
                        "Could not retrieve message due to a message exception.");

                    if (!ex.IsTransient)
                    {
                        if (brokeredMessage != null)
                        {
                            await brokeredMessage.DeadLetterAsync(
                                "message exception occurred and exception was not recoverable.",
                                ex.Message);
                        }
                        retryCount = 0; //reset the retry count as its a new message
                    }
                    else
                    {
                        retryCount++;
                    }
                }
                catch (SerializationException ex)
                {
                    _logger.Error(ex, "Failed to deserialise message. Dead lettering message.");

                    if (brokeredMessage != null)
                    {
                        await brokeredMessage.DeadLetterAsync(
                            "Message could not be deserialised.",
                            ex.Message);

                        retryCount = 0; //reset the retry count as its a new message
                    }
                    else
                    {
                        retryCount++;
                    }
                }
                catch  (Exception ex)
                {
                    _logger.Fatal(ex, "Unknown exception has occurred when getting a message.");
                    throw;
                }
            }
           
            if(!requestHandled)
            {
                _logger.Error(new Exception("Failed to get message off queue"), 
                    "Failed to get message off queue. Message is being dead lettered." );

                if(brokeredMessage != null)
                    await brokeredMessage.DeadLetterAsync("Failed to process message within retry limit.", "Message processing retry limit exceeded.");

                RefreshClient(); //We refresh the client for next time just in case the issue is related to connection as well.
            }

            return message;
        }
    }
}
