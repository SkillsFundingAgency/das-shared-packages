using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging.FileSystem
{
    public class FileSystemMessageSubscriber<T> : IMessageSubscriber<T> where T : new()
    {
        private readonly string _storageDirectory;
        private readonly string _topicName;
       
        public FileSystemMessageSubscriber(string storageDirectory, string topicName = "")
        {
            _storageDirectory = storageDirectory;
            _topicName = topicName;
        }

        public async Task<IMessage<T>> ReceiveAsAsync()
        {
            var nextFile = GetAvailableMessages(_topicName).FirstOrDefault();

            if (nextFile == null)
            {
                await Task.Delay(5000);
                return null;
            }

            return await FileSystemMessage<T>.Lock(nextFile);
        }

        public async Task<IEnumerable<IMessage<T>>> ReceiveBatchAsAsync(int batchSize)
        {
            var availableMessageFiles = GetAvailableMessages(_topicName).Take(batchSize).ToArray();
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

        public void Dispose()
        {
           //No clean up needed for file subscriber
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
    }
}
