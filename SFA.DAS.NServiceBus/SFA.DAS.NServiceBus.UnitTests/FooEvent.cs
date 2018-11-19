using System;

namespace SFA.DAS.NServiceBus.UnitTests
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