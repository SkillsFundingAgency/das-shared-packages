using System;
using NServiceBus;

namespace SFA.DAS.UnitOfWork.NServiceBus.UnitTests
{
    public class TestCommand : ICommand
    {
        public DateTime Created { get; }

        public TestCommand(DateTime created)
        {
            Created = created;
        }
    }
}