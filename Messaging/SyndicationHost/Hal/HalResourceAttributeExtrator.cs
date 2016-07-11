using System.Collections.Generic;
using SFA.DAS.Messaging.Syndication.Hal;

namespace SyndicationHost.Hal
{
    public class HalResourceAttributeExtrator : IHalResourceAttributeExtrator<Message>
    {
        public HalResourceAttributes Extract(Message resource)
        {
            return new HalResourceAttributes
            {
                Links = new Dictionary<string, string>
                {
                    { "self", "/messages/" + resource.Id }
                },
                Properties = new Dictionary<string, string>
                {
                    { "id", resource.Id }
                }
            };
        }
    }
}