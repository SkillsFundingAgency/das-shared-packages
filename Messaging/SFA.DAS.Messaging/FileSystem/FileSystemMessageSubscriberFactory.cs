using SFA.DAS.Messaging.Helpers;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.Messaging.FileSystem
{
    public class FileSystemMessageSubscriberFactory : IMessageSubscriberFactory
    {
        private readonly string _storageDirectory;

        public FileSystemMessageSubscriberFactory(string storageDirectory)
        {
            _storageDirectory = storageDirectory;
        }

        public IMessageSubscriber<T> GetSubscriber<T>() where T: new()
        {
            var messageGroupName = MessageGroupHelper.GetMessageGroupName<T>();

            return new FileSystemMessageSubscriber<T>(_storageDirectory, messageGroupName);
        }
    }
}
