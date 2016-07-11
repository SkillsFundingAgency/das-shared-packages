using Newtonsoft.Json;

namespace SFA.DAS.Messaging.Syndication.Hal
{
    public class HalPage
    {
        [JsonProperty("_links")]
        public HalPageLinks Links { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("_embedded")]
        public HalContent Embedded { get; set; }
    }
}