using System;
using System.IO;
using System.Threading.Tasks;

namespace SFA.DAS.Configuration.FileStorage
{
    public class FileStorageConfigurationRepository : IConfigurationRepository
    {
        public string Get(string serviceName, string environmentName, string version)
        {
            var path = GetConfigFilePath(serviceName, environmentName, version);

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                reader.Close();

                return json;
            }
        }

        private static string GetConfigFilePath(string serviceName, string environmentName, string version)
        {
            var appDataFolder = !string.IsNullOrEmpty((string)AppDomain.CurrentDomain.GetData("DataDirectory")) 
                ? (string)AppDomain.CurrentDomain.GetData("DataDirectory") 
                : Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "App_Data");

            var path = Path.Combine(appDataFolder, $"{environmentName}_{serviceName}_{version}.json");
            return path;
        }

        public async Task<string> GetAsync(string serviceName, string environmentName, string version)
        {
            var path = GetConfigFilePath(serviceName, environmentName, version);

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                var json = await reader.ReadToEndAsync();
                reader.Close();

                return json;
            }
        }
    }
}