using System;
using System.Data.Common;
using System.Threading.Tasks;
using Moq;
using NServiceBus.Persistence;
using NServiceBus.Persistence.Sql;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.NServiceBus.MsSqlServer;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.MsSqlServer
{
    [TestFixture]
    public class InvokeHandlerBehaviorTests : FluentTest<InvokeHandlerBehaviorTestsFixture>
    {
        [Test]
        public Task Invoke_WhenHandlingAMessage_ThenShouldSetUnitOfWorkContextDbConnectionBeforeNextTask()
        {
            return RunAsync(f => f.Invoke(), f => f.UnitOfWorkContext.Verify(c => c.Set(f.Connection.Object), Times.Once));
        }

        [Test]
        public Task Invoke_WhenALogicalMessageIsIncoming_ThenShouldSetUnitOfWorkContextDbTransactionBeforeNextTask()
        {
            return RunAsync(f => f.Invoke(), f => f.UnitOfWorkContext.Verify(c => c.Set(f.Transaction.Object), Times.Once));
        }
    }

    public class InvokeHandlerBehaviorTestsFixture : FluentTestFixture
    {
        public InvokeHandlerBehavior Behavior { get; set; }
        public TestableInvokeHandlerContext Context { get; set; }
        public FakeBuilder Builder { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public Mock<SynchronizedStorageSession> StorageSession { get; set; }
        public Mock<ISqlStorageSession> SqlStorageSession { get; set; }
        public Mock<DbConnection> Connection { get; set; }
        public Mock<DbTransaction> Transaction { get; set; }
        public bool NextTaskInvoked { get; set; }

        public InvokeHandlerBehaviorTestsFixture()
        {
            Behavior = new InvokeHandlerBehavior();
            Builder = new FakeBuilder();
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            StorageSession = new Mock<SynchronizedStorageSession>();
            SqlStorageSession = StorageSession.As<ISqlStorageSession>();
            Connection = new Mock<DbConnection>();
            Transaction = new Mock<DbTransaction>();

            Context = new TestableInvokeHandlerContext
            {
                Builder = Builder,
                SynchronizedStorageSession = StorageSession.Object
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

            SqlStorageSession.Setup(s => s.Connection).Returns(Connection.Object);
            SqlStorageSession.Setup(s => s.Transaction).Returns(Transaction.Object);
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