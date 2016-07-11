using Newtonsoft.Json;

namespace SFA.DAS.Messaging.Syndication.Hal
{
    public class HalPage<T>
    {
        [JsonProperty("_links")]
        public HalPageLinks Links { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("_embedded")]
        public HalContent<T> Embedded { get; set; }
    }
}