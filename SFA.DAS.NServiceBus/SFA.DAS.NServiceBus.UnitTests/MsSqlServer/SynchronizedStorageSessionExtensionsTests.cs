using System;
using FluentAssertions;
using Moq;
using NServiceBus.Persistence;
using NServiceBus.Persistence.Sql;
using NUnit.Framework;
using SFA.DAS.NServiceBus.MsSqlServer;

namespace SFA.DAS.NServiceBus.UnitTests.MsSqlServer
{
    [TestFixture]
    public class SynchronizedStorageSessionExtensionsTests : FluentTest<SynchronizedStorageSessionExtensionsTestsFixture>
    {
        [Test]
        public void GetSqlStorageSession_WhenGettingSqlStorageSession_TheShouldReturnSqlStorageSession()
        {
            Run(f => f.SetSqlStorageSession(), f => f.GetSqlStorageSession(), (f, r) => r.Should().Be(f.StorageSession.Object));
        }

        [Test]
        public void GetSqlStorageSession_WhenGettingSqlStorageSessionAndIsNotASqlStorageSession_TheShouldReturnSqlStorageSession()
        {
            Run(f => f.SetNonSqlStorageSession(), f => f.GetSqlStorageSession(), (f, a) => a.ShouldThrow<Exception>());
        }
    }

    public class SynchronizedStorageSessionExtensionsTestsFixture : FluentTestFixture
    {
        public Mock<SynchronizedStorageSession> StorageSession { get; set; }
        public Mock<ISqlStorageSession> SqlStorageSession { get; set; }

        public ISqlStorageSession GetSqlStorageSession()
        {
            return StorageSession.Object.GetSqlStorageSession();
        }

        public SynchronizedStorageSessionExtensionsTestsFixture SetSqlStorageSession()
        {
            StorageSession = new Mock<SynchronizedStorageSession>();
            SqlStorageSession = StorageSession.As<ISqlStorageSession>();

            return this;
        }

        public SynchronizedStorageSessionExtensionsTestsFixture SetNonSqlStorageSession()
        {
            StorageSession = new Mock<SynchronizedStorageSession>();

            return this;
        }
    }
}