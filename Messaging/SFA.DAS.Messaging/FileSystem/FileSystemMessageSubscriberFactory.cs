using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging.FileSystem
{
    public class FileSystemMessageSubscriberFactory<T> : IMessageSubscriberFactory<T> where T : new()
    {
        private readonly string _storageDirectory;
        private readonly string _topicName;

        public FileSystemMessageSubscriberFactory(string storageDirectory, string topicName)
        {
            _storageDirectory = storageDirectory;
            _topicName = topicName;
        }

        public IMessageSubscriber<T> GetSubscriber()
        {
            return new FileSystemMessageSubscriber<T>(_storageDirectory, _topicName);
        }
    }
}
