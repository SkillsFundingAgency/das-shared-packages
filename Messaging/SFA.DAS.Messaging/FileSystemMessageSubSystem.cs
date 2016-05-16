using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging
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

        public async Task<string> Dequeue()
        {
            var directory = new DirectoryInfo(_directoryPath);
            if (!directory.Exists)
            {
                return await Task.FromResult<string>(null);
            }

            var contents = directory.GetFiles("*.json", SearchOption.TopDirectoryOnly);
            if (!contents.Any())
            {
                return await Task.FromResult<string>(null);
            }

            var next = contents.OrderBy(fi => fi.LastWriteTimeUtc).First();
            string message;
            using (var stream = next.OpenRead())
            using (var reader = new StreamReader(stream))
            {
                message = await reader.ReadToEndAsync();
            }
            next.Delete();

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
    }
}
