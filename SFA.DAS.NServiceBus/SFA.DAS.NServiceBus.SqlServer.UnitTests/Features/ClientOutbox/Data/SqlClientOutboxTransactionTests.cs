using System.Data.Common;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.NServiceBus.SqlServer.Features.ClientOutbox.Data;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.SqlServer.UnitTests.Features.ClientOutbox.Data
{
    [TestFixture]
    public class SqlClientOutboxTransactionTests : FluentTest<SqlClientOutboxTransactionTestsFixture>
    {
        [Test]
        public void New_WhenCreatingASqlClientOutboxTransaction_ThenShouldCreateASqlClientOutboxTransaction()
        {
            Test(f => f.New(), (f, r) => r.Should().NotBeNull());
        }

        [Test]
        public void Transaction_WhenGettingTheConnection_ThenShouldReturnTheConnection()
        {
            Test(f => f.SetSqlClientOutboxTransaction(), f => f.SqlClientOutboxTransaction.Connection, (f, r) => r.Should().Be(f.Connection.Object));
        }

        [Test]
        public void Transaction_WhenGettingTheTransaction_ThenShouldReturnTheTransaction()
        {
            Test(f => f.SetSqlClientOutboxTransaction(), f => f.SqlClientOutboxTransaction.Transaction, (f, r) => r.Should().Be(f.Transaction.Object));
        }

        [Test]
        public Task CommitAsync_WhenCommitting_ThenShouldCommitTransaction()
        {
            return TestAsync(f => f.SetSqlClientOutboxTransaction(), f => f.CommitAsync(), f => f.Transaction.Verify(t => t.Commit(), Times.Once));
        }

        [Test]
        public void Dispose_WhenDisposing_ThenShouldDisposeTheTransaction()
        {
            Test(f => f.SetSqlClientOutboxTransaction(), f => f.Dispose(), f => f.Transaction.Protected().Verify("Dispose", Times.Once(), true));
        }

        [Test]
        public void Dispose_WhenDisposing_ThenShouldDisposeTheConnection()
        {
            Test(f => f.SetSqlClientOutboxTransaction(), f => f.Dispose(), f => f.Connection.Protected().Verify("Dispose", Times.Once(), true));
        }
    }

    public class SqlClientOutboxTransactionTestsFixture
    {
        public Mock<DbConnection> Connection { get; set; }
        public Mock<DbTransaction> Transaction { get; set; }
        public SqlClientOutboxTransaction SqlClientOutboxTransaction { get; set; }

        public SqlClientOutboxTransactionTestsFixture()
        {
            Connection = new Mock<DbConnection>();
            Transaction = new Mock<DbTransaction>();
        }

        public SqlClientOutboxTransaction New()
        {
            return new SqlClientOutboxTransaction(Connection.Object, Transaction.Object);
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