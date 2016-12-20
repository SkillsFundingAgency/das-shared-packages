using System;
using System.Net;
using Newtonsoft.Json;

namespace Sfa.Automation.Framework.Test
{
    public class TestJson
    {
        public string Other { get; set; }
        public string Evidence { get; set; }
        public string PluginId { get; set; }
        public string CweId { get; set; }
        public string Confidence { get; set; }
        public string WascId { get; set; }
        public string Description { get; set; }
        public string MessageId { get; set; }
        public string Url { get; set; }
        public string Reference { get; set; }
        public string Solution { get; set; }
        public string Alert { get; set; }
        public string Param { get; set; }
        public string Attack { get; set; }
        public string Name { get; set; }
        public string Risk { get; set; }
        public string Id { get; set; }

        public T DownloadSerializedJsonData<T>(string url) where T : new()
        {
            using (var w = new WebClient())
            {
                var jsonData = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    jsonData = w.DownloadString(url);
                }
                catch (Exception) { }
                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(jsonData) ? JsonConvert.DeserializeObject<T>(jsonData) : new T();
            }
        }
    }
}
