using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.Testing;
using SFA.DAS.UnitOfWork.Context;
using SFA.DAS.UnitOfWork.Managers;
using SFA.DAS.UnitOfWork.Pipeline;
using SFA.DAS.UnitOfWork.SqlServer.Managers;

namespace SFA.DAS.UnitOfWork.SqlServer.UnitTests.Managers
{
    [TestFixture]
    public class UnitOfWorkManagerTests : FluentTest<UnitOfWorkManagerTestsFixture>
    {
        [Test]
        public Task BeginAsync_WhenBeginning_ThenShouldOpenConnection()
        {
            return TestAsync(f => f.BeginAsync(), f => f.Connection.Verify(o => o.OpenAsync(CancellationToken.None), Times.Once));
        }

        [Test]
        public Task BeginAsync_WhenBeginning_ThenShouldBeginTransaction()
        {
            return TestAsync(f => f.BeginAsync(), f => f.Connection.Protected().Verify<DbTransaction>("BeginDbTransaction", Times.Once(), IsolationLevel.Unspecified));
        }

        [Test]
        public Task BeginAsync_WhenBeginning_ThenShouldSetUnitOfWorkContextConnection()
        {
            return TestAsync(f => f.BeginAsync(), f => f.UnitOfWorkContext.Verify(c => c.Set(f.Connection.Object), Times.Once));
        }

        [Test]
        public Task BeginAsync_WhenBeginning_ThenShouldSetUnitOfWorkContextTransaction()
        {
            return TestAsync(f => f.BeginAsync(), f => f.UnitOfWorkContext.Verify(c => c.Set(f.Transaction.Object), Times.Once));
        }

        [Test]
        public Task EndAsync_WhenEnding_ThenShouldCommitUnitsOfWork()
        {
            return TestAsync(f => f.BeginAsyncThenEndAsync(), f => f.UnitsOfWork.ForEach(u => u.Verify(u2 => u2.CommitAsync(It.IsAny<Func<Task>>()), Times.Once)));
        }

        [Test]
        public Task EndAsync_WhenEnding_ThenShouldCommitTransactionAfterCommittingUnitsOfWork()
        {
            return TestAsync(f => f.BeginAsyncThenEndAsync(), f => f.Transaction.Verify(t => t.Commit()));
        }

        [Test]
        public Task EndAsync_WhenEndingAfterAnException_ThenShouldNotCommitTransaction()
        {
            return TestAsync(f => f.BeginAsyncThenEndAsyncAfterException(), f => f.Transaction.Verify(t => t.Commit(), Times.Never));
        }

        [Test]
        public Task EndAsync_WhenEnding_ThenShouldDisposeTransaction()
        {
            return TestAsync(f => f.BeginAsyncThenEndAsync(), f => f.Transaction.Invocations.Any(x => x.Method.Name == "Dispose"));
        }

        [Test]
        public Task EndAsync_WhenEndingAfterAnException_ThenShouldDisposeTransaction()
        {
            return TestAsync(f => f.BeginAsyncThenEndAsyncAfterException(), f => f.Transaction.Invocations.Any(x=>x.Method.Name== "Dispose"));
        }
    }

    public class UnitOfWorkManagerTestsFixture
    {
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }
        public Mock<DbConnection> Connection { get; set; }
        public List<Mock<IUnitOfWork>> UnitsOfWork { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public Mock<DbTransaction> Transaction { get; set; }
        public int CommittedUnitsOfWork { get; set; }

        public UnitOfWorkManagerTestsFixture()
        {
            Connection = new Mock<DbConnection>();

            UnitsOfWork = new List<Mock<IUnitOfWork>>
            {
                new Mock<IUnitOfWork>(),
                new Mock<IUnitOfWork>(),
                new Mock<IUnitOfWork>()
            };

            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            Transaction = new Mock<DbTransaction> { CallBase = true };


            Transaction.Setup(t => t.Commit()).Callback(() =>
            {
                if (CommittedUnitsOfWork != UnitsOfWork.Count)
                    throw new Exception("CommitAsync called too early");
            });

            Connection.Protected().Setup<DbTransaction>("BeginDbTransaction", IsolationLevel.Unspecified).Returns(Transaction.Object);
            UnitsOfWork.ForEach(u => u.Setup(u2 => u2.CommitAsync(It.IsAny<Func<Task>>())).Returns<Func<Task>>(t => { CommittedUnitsOfWork++; return t(); }));

            UnitOfWorkManager = new UnitOfWorkManager(
                Connection.Object,
                UnitsOfWork.Select(u => u.Object),
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