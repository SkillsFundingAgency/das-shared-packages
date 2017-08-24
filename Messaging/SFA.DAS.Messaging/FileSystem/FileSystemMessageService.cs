using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Messaging.Attributes;

namespace SFA.DAS.Messaging.FileSystem
{
    public class FileSystemMessageService : IMessagePublisher, IPollingMessageReceiver
    {
        private readonly string _storageDirectory;
        private readonly string _queueName;

        public FileSystemMessageService(string storageDirectory, string queueName = "")
        {
            _storageDirectory = storageDirectory;
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

            var json = JsonConvert.SerializeObject(message);

            var directoryName = Path.Combine(_storageDirectory, !string.IsNullOrEmpty(_queueName) ? _queueName : queueName);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            var path = Path.Combine(directoryName, Guid.NewGuid() + ".json");
            using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(json);
                writer.Close();
            }
        }

        public async Task<IMessage<T>> ReceiveAsAsync<T>() where T : new()
        {
            var queueName = GetQueueName<T>();

            var nextFile = GetAvailableMessages(!string.IsNullOrEmpty(_queueName) ? _queueName : queueName).FirstOrDefault();
            if (nextFile == null)
            {
                await Task.Delay(5000);
                return null;
            }

            return await FileSystemMessage<T>.Lock(nextFile);
        }

        public async Task<IEnumerable<IMessage<T>>> ReceiveBatchAsAsync<T>(int batchSize) where T : new()
        {
            var queueName = GetQueueName<T>();

            var availableMessageFiles = GetAvailableMessages(!string.IsNullOrEmpty(_queueName) ? _queueName : queueName).Take(batchSize).ToArray();
            if (!availableMessageFiles.Any())
            {
                return new Message<T>[0];
            }

            var messages = new Message<T>[availableMessageFiles.Length];
            for (var i = 0; i < messages.Length; i++)
            {
                messages[i] = await FileSystemMessage<T>.Lock(availableMessageFiles[i]);
            }
            return messages;
        }


        private IEnumerable<FileInfo> GetAvailableMessages(string messagePath)
        {
            var directory = new DirectoryInfo(Path.Combine(_storageDirectory, messagePath));
            if (!directory.Exists)
            {
                return new FileInfo[0];
            }

            var jsonFiles = directory.GetFiles("*.json");
            var lockFiles = directory.GetFiles("*.lck");
            return jsonFiles
                .Where(jf => !lockFiles.Any(lf => lf.Name.StartsWith(jf.Name)))
                .OrderByDescending(jf => jf.LastWriteTimeUtc);
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
