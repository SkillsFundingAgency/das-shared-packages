using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NServiceBus.Persistence;
using NUnit.Framework;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.Testing;
using SFA.DAS.UnitOfWork.NServiceBus.ClientOutbox;

namespace SFA.DAS.UnitOfWork.NServiceBus.UnitTests.ClientOutbox
{
    [TestFixture]
    public class UnitOfWorkManagerTests : FluentTest<UnitOfWorkManagerTestsFixture>
    {
        [Test]
        public Task BeginAsync_WhenBeginning_ThenShouldBeginTransaction()
        {
            return RunAsync(f => f.BeginAsync(), f => f.ClientOutboxStorage.Verify(o => o.BeginTransactionAsync(), Times.Once));
        }

        [Test]
        public Task BeginAsync_WhenBeginning_ThenShouldSetUnitOfWorkContextTransaction()
        {
            return RunAsync(f => f.BeginAsync(), f => f.UnitOfWorkContext.Verify(c => c.Set(f.ClientOutboxTransaction.Object), Times.Once));
        }

        [Test]
        public Task BeginAsync_WhenBeginning_ThenShouldSetUnitOfWorkContextSynchronizedStorageSession()
        {
            return RunAsync(f => f.BeginAsync(), f => f.UnitOfWorkContext.Verify(c => c.Set(f.SynchronizedStorageSession.Object)));
        }

        [Test]
        public Task EndAsync_WhenEnding_ThenShouldCommitUnitsOfWork()
        {
            return RunAsync(f => f.BeginAsyncThenEndAsync(), f => f.UnitsOfWork.ForEach(u => u.Verify(u2 => u2.CommitAsync(It.IsAny<Func<Task>>()), Times.Once)));
        }

        [Test]
        public Task EndAsync_WhenEnding_ThenShouldCommitTransactionAfterCommittingUnitsOfWork()
        {
            return RunAsync(f => f.BeginAsyncThenEndAsync(), f => f.ClientOutboxTransaction.Verify(t => t.CommitAsync()));
        }

        [Test]
        public Task EndAsync_WhenEndingAfterAnException_ThenShouldNotCommitTransaction()
        {
            return RunAsync(f => f.BeginAsyncThenEndAsyncAfterException(), f => f.ClientOutboxTransaction.Verify(t => t.CommitAsync(), Times.Never));
        }

        [Test]
        public Task EndAsync_WhenEnding_ThenShouldDisposeTransaction()
        {
            return RunAsync(f => f.BeginAsyncThenEndAsync(), f => f.ClientOutboxTransaction.Verify(t => t.Dispose(), Times.Once));
        }

        [Test]
        public Task EndAsync_WhenEndingAfterAnException_ThenShouldDisposeTransaction()
        {
            return RunAsync(f => f.BeginAsyncThenEndAsyncAfterException(), f => f.ClientOutboxTransaction.Verify(t => t.Dispose(), Times.Once));
        }
    }

    public class UnitOfWorkManagerTestsFixture
    {
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }
        public Mock<IClientOutboxStorageV2> ClientOutboxStorage { get; set; }
        public List<Mock<IUnitOfWork>> UnitsOfWork { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public Mock<IClientOutboxTransaction> ClientOutboxTransaction { get; set; }
        public Mock<SynchronizedStorageSession> SynchronizedStorageSession { get; set; }
        public int CommittedUnitsOfWork { get; set; }

        public UnitOfWorkManagerTestsFixture()
        {
            ClientOutboxStorage = new Mock<IClientOutboxStorageV2>();

            UnitsOfWork = new List<Mock<IUnitOfWork>>
            {
                new Mock<IUnitOfWork>(),
                new Mock<IUnitOfWork>(),
                new Mock<IUnitOfWork>()
            };

            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            ClientOutboxTransaction = new Mock<IClientOutboxTransaction>();
            SynchronizedStorageSession = ClientOutboxTransaction.As<SynchronizedStorageSession>();

            ClientOutboxTransaction.Setup(t => t.CommitAsync()).Callback(() =>
            {
                if (CommittedUnitsOfWork != UnitsOfWork.Count)
                    throw new Exception("CommitAsync called too early");
            });

            ClientOutboxStorage.Setup(o => o.BeginTransactionAsync()).ReturnsAsync(ClientOutboxTransaction.Object);
            UnitsOfWork.ForEach(u => u.Setup(u2 => u2.CommitAsync(It.IsAny<Func<Task>>())).Returns<Func<Task>>(t => { CommittedUnitsOfWork++; return t(); }));

            UnitOfWorkManager = new UnitOfWorkManager(
                ClientOutboxStorage.Object,
                UnitsOfWork.Select(u => u.Object).Concat(new [] { new UnitOfWork(null, null) }),
                UnitOfWorkContext.Object);
        }

        public Task BeginAsync()
        {
            return UnitOfWorkManager.BeginAsync();
        }

        public async Task BeginAsyncThenEndAsync()
        {
            await UnitOfWorkManager.BeginAsync();
            await UnitOfWorkManager.EndAsync();
        }

        public async Task BeginAsyncThenEndAsyncAfterException()
        {
            await UnitOfWorkManager.BeginAsync();
            await UnitOfWorkManager.EndAsync(new Exception());
        }
    }
}