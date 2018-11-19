using System;

namespace SFA.DAS.UnitOfWork.NServiceBus.UnitTests
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