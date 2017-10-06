using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.Messaging.FileSystem
{
    public class FileSystemMessagePublisher<T> : IMessagePublisher<T> where T : new()
    {
        private readonly string _storageDirectory;
        private readonly string _topicName;

        public FileSystemMessagePublisher(string storageDirectory, string topicName = "")
        {
            _storageDirectory = storageDirectory;
            _topicName = topicName;
        }

        public async Task PublishAsync(T message)
        {
            var json = JsonConvert.SerializeObject(message);

            var directoryName = Path.Combine(_storageDirectory, _topicName);
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
    }
}
