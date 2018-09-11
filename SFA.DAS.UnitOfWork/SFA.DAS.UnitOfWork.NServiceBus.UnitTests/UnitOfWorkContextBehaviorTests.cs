using System;
using System.Data.Common;
using System.Threading.Tasks;
using Moq;
using NServiceBus.Persistence;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.UnitOfWork.NServiceBus.UnitTests
{
    [TestFixture]
    public class UnitOfWorkContextBehaviorTests : FluentTest<InvokeHandlerBehaviorTestsFixture>
    {
        [Test]
        public Task Invoke_WhenInvokingAHandler_ThenShouldSetUnitOfWorkContextSynchronizedStorageSessionBeforeNextTask()
        {
            return RunAsync(f => f.Invoke(), f => f.UnitOfWorkContext.Verify(c => c.Set(f.SynchronizedStorageSession.Object), Times.Once));
        }
    }

    public class InvokeHandlerBehaviorTestsFixture : FluentTestFixture
    {
        public UnitOfWorkContextBehavior Behavior { get; set; }
        public TestableInvokeHandlerContext Context { get; set; }
        public FakeBuilder Builder { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public Mock<SynchronizedStorageSession> SynchronizedStorageSession { get; set; }
        public bool NextTaskInvoked { get; set; }

        public InvokeHandlerBehaviorTestsFixture()
        {
            Behavior = new UnitOfWorkContextBehavior();
            Builder = new FakeBuilder();
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            SynchronizedStorageSession = new Mock<SynchronizedStorageSession>();

            Context = new TestableInvokeHandlerContext
            {
                Builder = Builder,
                SynchronizedStorageSession = SynchronizedStorageSession.Object
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