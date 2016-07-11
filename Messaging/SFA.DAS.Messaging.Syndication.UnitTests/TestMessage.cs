using System;

namespace SFA.DAS.Messaging.Syndication.UnitTests
{
    public class TestMessage
    {
        public TestMessage()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
    }
}
