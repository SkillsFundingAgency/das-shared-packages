using System;

namespace SFA.DAS.NServiceBus.SqlServer.UnitTests
{
    public class FooEvent
    {
        public DateTime Created { get; }

        public FooEvent(DateTime created)
        {
            Created = created;
        }
    }
}