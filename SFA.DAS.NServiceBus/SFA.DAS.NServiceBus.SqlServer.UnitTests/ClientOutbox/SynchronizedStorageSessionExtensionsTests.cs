using System;
using FluentAssertions;
using Moq;
using NServiceBus.Persistence;
using NServiceBus.Persistence.Sql;
using NUnit.Framework;
using SFA.DAS.NServiceBus.SqlServer.ClientOutbox;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.SqlServer.UnitTests.ClientOutbox
{
    [TestFixture]
    public class SynchronizedStorageSessionExtensionsTests : FluentTest<SynchronizedStorageSessionExtensionsTestsFixture>
    {
        [Test]
        public void GetSqlSession_WhenGettingTheSqlSession_TheShouldReturnTheSqlSession()
        {
            Run(f => f.SetSqlSession(), f => f.GetSqlSession(), (f, r) => r.Should().Be(f.Session.Object));
        }

        [Test]
        public void GetSqlSession_WhenGettingTheSqlSessionAndItIsNotASqlStorageSession_ThenShouldThrowAnException()
        {
            Run(f => f.SetNonSqlSession(), f => f.GetSqlSession(), (f, a) => a.ShouldThrow<Exception>()
                .WithMessage("Cannot access the SQL session"));
        }
    }

    public class SynchronizedStorageSessionExtensionsTestsFixture : FluentTestFixture
    {
        public Mock<SynchronizedStorageSession> Session { get; set; }
        public Mock<ISqlStorageSession> SqlSession { get; set; }

        public ISqlStorageSession GetSqlSession()
        {
            return Session.Object.GetSqlSession();
        }

        public SynchronizedStorageSessionExtensionsTestsFixture SetSqlSession()
        {
            Session = new Mock<SynchronizedStorageSession>();
            SqlSession = Session.As<ISqlStorageSession>();

            return this;
        }

        public SynchronizedStorageSessionExtensionsTestsFixture SetNonSqlSession()
        {
            Session = new Mock<SynchronizedStorageSession>();

            return this;
        }
    }
}