using System.Collections.Generic;

namespace SFA.DAS.Messaging.Syndication.Hal
{
    public class HalResponse
    {
        public Dictionary<string, string[]> Headers { get; set; }
        public string Content { get; set; }
    }
}