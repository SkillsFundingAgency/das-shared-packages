using System;

namespace SFA.DAS.UnitOfWork.NServiceBus.UnitTests
{
    public class BarEvent
    {
        public DateTime Created { get; }

        public BarEvent(DateTime created)
        {
            Created = created;
        }
    }
}