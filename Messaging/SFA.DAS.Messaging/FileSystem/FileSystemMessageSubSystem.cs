using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging.FileSystem
{
    public class FileSystemMessageSubSystem : IMessageSubSystem
    {
        private readonly string _directoryPath;

        public FileSystemMessageSubSystem()
            : this(GetDefaultDirectoryPath())
        {
        }
        public FileSystemMessageSubSystem(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        public async Task Enqueue(string message)
        {
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }

            var path = Path.Combine(_directoryPath, Guid.NewGuid() + ".json");
            using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(message);
                await writer.FlushAsync();
                writer.Close();
            }
        }

        public async Task<SubSystemMessage> Dequeue()
        {
            DeleteOldLockFiles();
            var messages = GetDirectoryContents("*.json");
            var locks = GetDirectoryContents("*.lck");
            var availableMessages = messages.Where(m => !locks.Any(l => l.Name.StartsWith(m.Name)));

            if (!availableMessages.Any())
            {
                return null;
            }

            var next = availableMessages.OrderBy(fi => fi.LastWriteTimeUtc).First();
            var message = await FileSystemMessage.Create(next);
            return message;
        }


        private static string GetDefaultDirectoryPath()
        {
            var appSettingsPath = ConfigurationManager.AppSettings["DAS:Messaging:QueuePath"];
            if (!string.IsNullOrEmpty(appSettingsPath))
            {
                return appSettingsPath;
            }
            return Path.Combine(Environment.GetEnvironmentVariable("TEMP"), Guid.NewGuid().ToString());
        }
        private IEnumerable<FileInfo> GetDirectoryContents(string filter = "*.*")
        {
            var directory = new DirectoryInfo(_directoryPath);
            if (!directory.Exists)
            {
                return new FileInfo[0];
            }

            var contents = directory.GetFiles(filter, SearchOption.TopDirectoryOnly);
            if (!contents.Any())
            {
                return new FileInfo[0];
            }

            return contents;
        }
        private void DeleteOldLockFiles()
        {
            var contents = GetDirectoryContents("*.lck");
            var oldFiles = contents.Where(f => f.LastWriteTimeUtc.CompareTo(DateTime.UtcNow.AddMinutes(-10)) < 0);
            foreach (var file in oldFiles)
            {
                file.Delete();
            }
        }
    }
}
