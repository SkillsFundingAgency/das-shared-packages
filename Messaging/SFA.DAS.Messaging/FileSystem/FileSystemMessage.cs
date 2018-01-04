using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SFA.DAS.Messaging.FileSystem
{
    public class FileSystemMessage<T> : Message<T>
    {
        private readonly FileInfo _dataFile;
        private readonly FileInfo _lockFile;

        public FileSystemMessage(FileInfo dataFile, FileInfo lockFile, T content) : base(content)
        {
            _dataFile = dataFile;
            _lockFile = lockFile;
        }

        public override Task CompleteAsync()
        {
            _dataFile.Delete();
            _lockFile.Delete();
            return Task.FromResult<object>(null);
        }

        public override Task AbortAsync()
        {
            _lockFile.Delete();
            return Task.FromResult<object>(null);
        }


        public static async Task<FileSystemMessage<T>> Lock(FileInfo dataFile)
        {
            var lockFile = new FileInfo(Path.Combine(dataFile.Directory.FullName, dataFile.Name + ".lck"));
            using (var s = lockFile.Create())
            {
                s.Close();
            }

            using (var stream = new FileStream(dataFile.FullName, FileMode.Open, FileAccess.Read))

            using (var reader = new StreamReader(stream))
            {
                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new PrivateSetterJsonContractResolver()
                };

                var json = await reader.ReadToEndAsync();
                var content = JsonConvert.DeserializeObject<T>(json, jsonSettings);

                return new FileSystemMessage<T>(dataFile, lockFile, content);
            }
        }

        internal class PrivateSetterJsonContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(
                MemberInfo member,
                MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);
                var property = member as PropertyInfo;

                if (prop.Writable || property == null)
                    return prop;

                var hasPrivateSetter = property.GetSetMethod(true) != null;
                prop.Writable = hasPrivateSetter;

                return prop;
            }
        }
    }
}