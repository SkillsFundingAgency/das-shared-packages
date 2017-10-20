using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Messaging.Helpers;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging.FileSystem
{
    public class FileSystemMessagePublisher : IMessagePublisher
    {
        private readonly string _storageDirectory;

        public FileSystemMessagePublisher(string storageDirectory)
        {
            _storageDirectory = storageDirectory;
        }

        public async Task PublishAsync(object message)
        {
            var messageGroupName = MessageGroupHelper.GetMessageGroupName(message);

            var json = JsonConvert.SerializeObject(message);

            var directoryName = Path.Combine(_storageDirectory, messageGroupName);
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
