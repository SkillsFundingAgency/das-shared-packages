using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.Messaging.FileSystem
{
    public class FileSystemMessageService : IMessagePublisher, IPollingMessageReceiver
    {
        private readonly string _storageDirectory;

        public FileSystemMessageService(string storageDirectory)
        {
            _storageDirectory = storageDirectory;
            if (!Directory.Exists(_storageDirectory))
            {
                Directory.CreateDirectory(_storageDirectory);
            }
        }


        public async Task PublishAsync(object message)
        {
            var json = JsonConvert.SerializeObject(message);

            if (!Directory.Exists(_storageDirectory))
            {
                Directory.CreateDirectory(_storageDirectory);
            }

            var path = Path.Combine(_storageDirectory, Guid.NewGuid() + ".json");
            using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(json);
                writer.Close();
            }
        }

        public async Task<IMessage<T>> ReceiveAsAsync<T>() where T : new()
        {
            var nextFile = GetAvailableMessages().FirstOrDefault();
            if (nextFile == null)
            {
                return null;
            }

            return await FileSystemMessage<T>.Lock(nextFile);
        }
        public async Task<IEnumerable<IMessage<T>>> ReceiveBatchAsAsync<T>(int batchSize) where T : new()
        {
            var availableMessageFiles = GetAvailableMessages().Take(batchSize).ToArray();
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


        private IEnumerable<FileInfo> GetAvailableMessages()
        {
            var directory = new DirectoryInfo(_storageDirectory);
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
    }
}
