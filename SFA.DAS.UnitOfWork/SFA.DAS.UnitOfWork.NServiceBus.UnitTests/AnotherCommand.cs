using System;
using NServiceBus;

namespace SFA.DAS.UnitOfWork.NServiceBus.UnitTests
{
    public class AnotherCommand
    {
        public DateTime Created { get; }

        public AnotherCommand(DateTime created)
        {
            Created = created;
        }
    }
}