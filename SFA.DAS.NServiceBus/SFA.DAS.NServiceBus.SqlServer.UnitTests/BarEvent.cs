using System;

namespace SFA.DAS.NServiceBus.SqlServer.UnitTests
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