using System;
using System.IO;
using SFA.DAS.Messaging.FileSystem;

namespace PubSubSampleGui.SubSystems
{
    public class FileSystemSubSystem : SubSystemBase
    {
        public FileSystemSubSystem(string storageDir = null)
        {
            var publisher = new FileSystemMessagePublisher(storageDir ?? GetDefaultStorageDir());
            var subscriberFactory = new FileSystemMessageSubscriberFactory(storageDir ?? GetDefaultStorageDir());

            Init(publisher, subscriberFactory);
        }

        private static string GetDefaultStorageDir()
        {
            return Path.Combine(Environment.GetEnvironmentVariable("TEMP"), Guid.NewGuid().ToString());
        }
    }
}
