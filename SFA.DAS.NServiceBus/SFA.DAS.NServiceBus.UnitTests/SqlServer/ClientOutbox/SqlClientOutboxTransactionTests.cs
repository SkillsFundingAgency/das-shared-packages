using System.Data.Common;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.NServiceBus.SqlServer.ClientOutbox;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.SqlServer.ClientOutbox
{
    [TestFixture]
    public class SqlClientOutboxTransactionTests : FluentTest<SqlClientOutboxTransactionTestsFixture>
    {
        [Test]
        public void New_WhenCreatingAnOutboxTransaction_ThenShouldCreateAnOutboxTransaction()
        {
            Run(f => f.New(), (f, r) => r.Should().NotBeNull());
        }
        [Test]
        public void Transaction_WhenGettingTheTransaction_ThenShouldReturnTheTransaction()
        {
            Run(f => f.SetSqlClientOutboxTransaction(), f => f.SqlClientOutboxTransaction.Transaction, (f, r) => r.Should().Be(f.Transaction.Object));
        }

        [Test]
        public Task CommitAsync_WhenCommitting_ThenShouldCommitTransaction()
        {
            return RunAsync(f => f.SetSqlClientOutboxTransaction(), f => f.CommitAsync(), f => f.Transaction.Verify(t => t.Commit(), Times.Once));
        }

        [Test]
        public void Dispose_WhenDisposing_ThenShouldDisposeTheTransaction()
        {
            Run(f => f.SetSqlClientOutboxTransaction(), f => f.Dispose(), f => f.Transaction.Protected().Verify("Dispose", Times.Once(), true));
        }
    }

    public class SqlClientOutboxTransactionTestsFixture : FluentTestFixture
    {
        public Mock<DbTransaction> Transaction { get; set; }
        public SqlClientOutboxTransaction SqlClientOutboxTransaction { get; set; }

        public SqlClientOutboxTransactionTestsFixture()
        {
            Transaction = new Mock<DbTransaction>();
        }

        public SqlClientOutboxTransaction New()
        {
            return new SqlClientOutboxTransaction(Transaction.Object);
        }

        public Task CommitAsync()
        {
            return SqlClientOutboxTransaction.CommitAsync();
        }

        public void Dispose()
        {
            SqlClientOutboxTransaction.Dispose();
        }

        public SqlClientOutboxTransactionTestsFixture SetSqlClientOutboxTransaction()
        {
            SqlClientOutboxTransaction = New();

            return this;
        }
    }
}