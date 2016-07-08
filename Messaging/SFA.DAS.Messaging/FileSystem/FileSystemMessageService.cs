using System;
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

        public async Task<Message<T>> ReceiveAsAsync<T>() where T : new()
        {
            var directory = new DirectoryInfo(_storageDirectory);
            if (!directory.Exists)
            {
                return null;
            }

            var jsonFiles = directory.GetFiles("*.json");
            var lockFiles = directory.GetFiles("*.lck");
            var nextFile = jsonFiles
                .Where(jf => !lockFiles.Any(lf => lf.Name.StartsWith(jf.Name)))
                .OrderByDescending(jf => jf.LastWriteTimeUtc)
                .FirstOrDefault();
            if (nextFile == null)
            {
                return null;
            }

            return await FileSystemMessage<T>.Lock(nextFile);
        }
    }
}
