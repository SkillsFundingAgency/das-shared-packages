using System;
using System.Data.Common;
using System.Threading.Tasks;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.NServiceBus.MsSqlServer;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.MsSqlServer
{
    [TestFixture]
    public class IncomingPhysicalMessageBehaviorTests : FluentTest<IncomingPhysicalMessageBehaviorTestsFixture>
    {
        [Test]
        public Task Invoke_WhenHandlingAnIncomingPhysicalMessage_ThenShouldSetUnitOfWorkContextDbConnectionBeforeNextTask()
        {
            return RunAsync(f => f.Invoke(), f => f.UnitOfWorkContext.Verify(c => c.Set<DbConnection>(null), Times.Once));
        }

        [Test]
        public Task Invoke_WhenHandlingAnIncomingPhysicalMessage_ThenShouldSetUnitOfWorkContextDbTransactionBeforeNextTask()
        {
            return RunAsync(f => f.Invoke(), f => f.UnitOfWorkContext.Verify(c => c.Set<DbTransaction>(null), Times.Once));
        }
    }

    public class IncomingPhysicalMessageBehaviorTestsFixture : FluentTestFixture
    {
        public IncomingPhysicalMessageBehavior Behavior { get; set; }
        public TestableIncomingPhysicalMessageContext Context { get; set; }
        public FakeBuilder Builder { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public bool NextTaskInvoked { get; set; }

        public IncomingPhysicalMessageBehaviorTestsFixture()
        {
            Behavior = new IncomingPhysicalMessageBehavior();
            Builder = new FakeBuilder();
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();

            Context = new TestableIncomingPhysicalMessageContext
            {
                Builder = Builder
            };

            UnitOfWorkContext.Setup(c => c.Set<DbConnection>(null)).Callback<DbConnection>(t =>
            {
                if (NextTaskInvoked)
                    throw new Exception("Set called too late");
            });

            UnitOfWorkContext.Setup(c => c.Set<DbTransaction>(null)).Callback<DbTransaction>(t =>
            {
                if (NextTaskInvoked)
                    throw new Exception("Set called too late");
            });

            Builder.Register(UnitOfWorkContext.Object);
        }

        public Task Invoke()
        {
            return Behavior.Invoke(Context, () =>
            {
                NextTaskInvoked = true;

                return Task.CompletedTask;
            });
        }
    }
}