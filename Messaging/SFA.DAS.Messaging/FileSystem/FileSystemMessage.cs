using System.IO;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging.FileSystem
{
    public class FileSystemMessage : SubSystemMessage
    {
        private readonly FileInfo _file;

        private FileSystemMessage(FileInfo file, string content)
        {
            _file = file;
            Content = content;
        }

        public override Task CompleteAsync()
        {
            var lockPath = GetLockPath(_file);
            _file.Delete();
            File.Delete(lockPath);
            return Task.FromResult<object>(null);
        }

        public override Task AbortAsync()
        {
            var lockPath = GetLockPath(_file);
            File.Delete(lockPath);
            return Task.FromResult<object>(null);
        }

        public static async Task<FileSystemMessage> Create(FileInfo file)
        {
            var lockFilePath = GetLockPath(file);

            // TODO: Handle error when lock file exists
            using (var stream = new FileStream(lockFilePath, FileMode.CreateNew, FileAccess.Write))
            {
                stream.Close();
            }

            string content;
            using (var stream = file.OpenRead())
            using (var reader = new StreamReader(stream))
            {
                content = await reader.ReadToEndAsync();
            }

            return new FileSystemMessage(file, content);
        }

        private static string GetLockPath(FileInfo file)
        {
            return file.FullName + ".lck";
        }
    }
}
