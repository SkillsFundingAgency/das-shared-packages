using System.Collections.Generic;

namespace SFA.DAS.Messaging.Syndication.Hal
{
    public class HalResourceAttributes
    {
        public Dictionary<string, string> Links { get; set; }
        public Dictionary<string,string> Properties { get; set; }
    }
}
