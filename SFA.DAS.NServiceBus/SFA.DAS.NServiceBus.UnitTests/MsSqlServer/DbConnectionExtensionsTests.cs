using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.NServiceBus.MsSqlServer;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.MsSqlServer
{
    [TestFixture]
    public class DbConnectionExtensionsTests : FluentTest<DbConnectionExtensionsTestsFixture>
    {
        [Test]
        public Task TryOpenAsync_WhenOpeningAConnection_ThenShouldOpenTheConnection()
        {
            return RunAsync(f => f.TryOpenAsync(), f => f.Connection.Verify(c => c.OpenAsync(CancellationToken.None), Times.Once()));
        }

        [Test]
        public Task TryOpenAsync_WhenOpeningAConnectionAndAnExceptionIsThrown_ThenShouldDisposeTheConnection()
        {
            return RunAsync(f => f.SetupThrowException(), f => f.TryOpenAsyncAndSwallowException(), f => f.Connection.Protected().Verify("Dispose", Times.Once(), true));
        }

        [Test]
        public Task TryOpenAsync_WhenOpeningAConnectionAndAnExceptionIsThrown_ThenShouldRethrowTheException()
        {
            return RunAsync(f => f.SetupThrowException(), f => f.TryOpenAsync(), (f, a) => a.ShouldThrow<Exception>());
        }
    }

    public class DbConnectionExtensionsTestsFixture : FluentTestFixture
    {
        public Mock<DbConnection> Connection { get; set; }

        public DbConnectionExtensionsTestsFixture()
        {
            Connection = new Mock<DbConnection>();
        }

        public Task TryOpenAsync()
        {
            return Connection.Object.TryOpenAsync();
        }

        public async Task TryOpenAsyncAndSwallowException()
        {
            try
            {
                await Connection.Object.TryOpenAsync();
            }
            catch
            {
            }
        }

        public DbConnectionExtensionsTestsFixture SetupThrowException()
        {
            Connection.Setup(c => c.OpenAsync(CancellationToken.None)).Throws(new Exception());

            return this;
        }
    }
}