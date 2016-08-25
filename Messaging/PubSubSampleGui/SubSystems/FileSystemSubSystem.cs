using System;
using System.IO;
using SFA.DAS.Messaging.FileSystem;

namespace PubSubSampleGui.SubSystems
{
    public class FileSystemSubSystem : SubSystemBase
    {
        private readonly FileSystemMessageService _fileSystemService;

        public FileSystemSubSystem(string storageDir = null)
        {
            _fileSystemService = new FileSystemMessageService(storageDir ?? GetDefaultStorageDir());
            Init(_fileSystemService, _fileSystemService);
        }

        private string GetDefaultStorageDir()
        {
            return Path.Combine(Environment.GetEnvironmentVariable("TEMP"), Guid.NewGuid().ToString());
        }
    }
}
