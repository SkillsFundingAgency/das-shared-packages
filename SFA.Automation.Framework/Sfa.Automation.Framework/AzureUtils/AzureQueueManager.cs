using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace Sfa.Automation.Framework.AzureUtils
{
    public class AzureQueueManager
    {
        private readonly CloudQueue _queue;

        /// <summary>
        /// Create a connection to the Azure Queue and connects to the requested Queue.
        /// </summary>
        /// <param name="connectionString">The connection string to connect to Azure Queue</param>
        /// <param name="queueName">The Azure Queue to connect to</param>
        public AzureQueueManager(string connectionString, string queueName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            queueClient.DefaultRequestOptions.RetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(5), 3);
            _queue = queueClient.GetQueueReference(queueName);
        }

        /// <summary>
        /// Adds a message to the Queue
        /// </summary>
        /// <param name="message">The message to be added</param>
        public void AddMessage(CloudQueueMessage message)
        {
            _queue.AddMessage(message);
        }

        /// <summary>
        /// Peeks a single message from the Queue
        /// </summary>
        /// <returns>The message</returns>
        public CloudQueueMessage PeekMessage()
        {
            return _queue.PeekMessage();
        }

        /// <summary>
        /// Gets a single message from teh Queue
        /// </summary>
        /// <returns>The message</returns>
        public CloudQueueMessage GetMessage()
        {
            return _queue.GetMessage();
        }

        /// <summary>
        /// Clears all messages from the Queue
        /// </summary>
        public void ClearQueue()
        {
            _queue.Clear();
        }

        /// <summary>
        /// Gets the approximate message count from the Queue
        /// </summary>
        /// <returns>The number of messages</returns>
        public int? GetApproximateMessageCount()
        {
            _queue.FetchAttributes();
            return _queue.ApproximateMessageCount;
        }

        /// <summary>
        /// Gets the specified number of messages from the Queue
        /// </summary>
        /// <param name="messageCount">Number of messages to get</param>
        /// <param name="fromMinutes"></param>
        /// <returns></returns>
        public IEnumerable<CloudQueueMessage> GetMessages(int messageCount, TimeSpan fromMinutes)
        {
            return _queue.GetMessages(messageCount, fromMinutes);
        }

        /// <summary>
        /// Deletes a message
        /// </summary>
        /// <param name="message">Message to be deleted</param>
        public void DeleteMessage(CloudQueueMessage message)
        {
            _queue.DeleteMessage(message);
        }

        /// <summary>
        /// Updates a message
        /// </summary>
        /// <param name="messageText">Updated message</param>
        public void UpdateMessage(string messageText)
        {
            CloudQueueMessage message = _queue.GetMessage();
            message.SetMessageContent(messageText);
            _queue.UpdateMessage(message, TimeSpan.FromSeconds(0.0), MessageUpdateFields.Content | MessageUpdateFields.Visibility);
        }
    }
}
