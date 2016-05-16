using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging.UnitTests
{
    public class TestEvent
    {
        public DateTimeOffset Timestamp { get; set; }

        public static TestEvent GetDefault()
        {
            return new TestEvent
            {
                Timestamp = DateTimeOffset.Now
            };
        }
    }
}
