using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.Testing;
using SFA.DAS.UnitOfWork.NServiceBus.ClientOutbox;

namespace SFA.DAS.UnitOfWork.NServiceBus.UnitTests.ClientOutbox
{
    [TestFixture]
    public class UnitOfWorkManagerTests : FluentTest<UnitOfWorkManagerTestsFixture>
    {
        [Test]
        public Task BeginAsync_WhenBeginningAUnitOfWork_ThenShouldBeginTransaction()
        {
            return RunAsync(f => f.BeginAsync(), f => f.ClientOutboxStorage.Verify(o => o.BeginTransactionAsync(), Times.Once));
        }

        [Test]
        public Task EndAsync_WhenEndingAUnitOfWork_ThenShouldCommitTransaction()
        {
            return RunAsync(f => f.BeginAsyncThenEndAsync(), f => f.ClientOutboxTransaction.Verify(t => t.CommitAsync()));
        }

        [Test]
        public Task EndAsync_WhenEndingAUnitOfWorkAfterAnException_ThenShouldNotCommitTransaction()
        {
            return RunAsync(f => f.BeginAsyncThenEndAsyncAfterException(), f => f.ClientOutboxTransaction.Verify(t => t.CommitAsync(), Times.Never));
        }

        [Test]
        public Task EndAsync_WhenEndingAUnitOfWork_ThenShouldDisposeTransaction()
        {
            return RunAsync(f => f.BeginAsyncThenEndAsync(), f => f.ClientOutboxTransaction.Verify(t => t.Dispose(), Times.Once));
        }

        [Test]
        public Task EndAsync_WhenEndingAUnitOfWorkAfterAnException_ThenShouldDisposeTransaction()
        {
            return RunAsync(f => f.BeginAsyncThenEndAsyncAfterException(), f => f.ClientOutboxTransaction.Verify(t => t.Dispose(), Times.Once));
        }
    }

    public class UnitOfWorkManagerTestsFixture : FluentTestFixture
    {
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }
        public Mock<IClientOutboxStorage> ClientOutboxStorage { get; set; }
        public List<IUnitOfWork> UnitsOfWork { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public Mock<IClientOutboxTransaction> ClientOutboxTransaction { get; set; }
        
        public UnitOfWorkManagerTestsFixture()
        {
            ClientOutboxStorage = new Mock<IClientOutboxStorage>();
            UnitsOfWork = new List<IUnitOfWork>();
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            ClientOutboxTransaction = new Mock<IClientOutboxTransaction>();
            
            ClientOutboxStorage.Setup(o => o.BeginTransactionAsync()).ReturnsAsync(ClientOutboxTransaction.Object);

            UnitOfWorkManager = new UnitOfWorkManager(
                ClientOutboxStorage.Object,
                UnitsOfWork,
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