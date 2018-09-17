using System;
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
        public Mock<Func<Task>> NextTask { get; set; }
        public bool NextTaskInvoked { get; set; }

        public InvokeHandlerBehaviorTestsFixture()
        {
            Behavior = new UnitOfWorkContextBehavior();
            Builder = new FakeBuilder();
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            SynchronizedStorageSession = new Mock<SynchronizedStorageSession>();
            NextTask = new Mock<Func<Task>>();

            Context = new TestableInvokeHandlerContext
            {
                Builder = Builder,
                SynchronizedStorageSession = SynchronizedStorageSession.Object
            };

            UnitOfWorkContext.Setup(c => c.Set(It.IsAny<SynchronizedStorageSession>())).Callback<SynchronizedStorageSession>(s =>
            {
                if (NextTaskInvoked)
                    throw new Exception("Set called too late");
            });

            NextTask.Setup(n => n()).Returns(Task.CompletedTask).Callback(() => NextTaskInvoked = true);

            Builder.Register(UnitOfWorkContext.Object);
        }
        
        public Task Invoke()
        {
            return Behavior.Invoke(Context, NextTask.Object);
        }
    }
}