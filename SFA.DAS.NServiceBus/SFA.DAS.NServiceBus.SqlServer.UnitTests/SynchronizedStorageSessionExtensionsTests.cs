using System;
using FluentAssertions;
using Moq;
using NServiceBus.Persistence;
using NServiceBus.Persistence.Sql;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.SqlServer.UnitTests
{
    [TestFixture]
    public class SynchronizedStorageSessionExtensionsTests : FluentTest<SynchronizedStorageSessionExtensionsTestsFixture>
    {
        [Test]
        public void GetSqlStorageSession_WhenGettingTheSqlSession_TheShouldReturnTheSqlSession()
        {
            Run(f => f.SetSqlSession(), f => f.GetSqlStorageSession(), (f, r) => r.Should().Be(f.Session.Object));
        }

        [Test]
        public void GetSqlStorageSession_WhenGettingTheSqlSessionAndItIsNotASqlStorageSession_ThenShouldThrowAnException()
        {
            Run(f => f.SetNonSqlSession(), f => f.GetSqlStorageSession(), (f, a) => a.Should().Throw<Exception>()
                .WithMessage("Cannot access the SQL storage session"));
        }
    }

    public class SynchronizedStorageSessionExtensionsTestsFixture
    {
        public Mock<SynchronizedStorageSession> Session { get; set; }
        public Mock<ISqlStorageSession> SqlSession { get; set; }

        public ISqlStorageSession GetSqlStorageSession()
        {
            return Session.Object.GetSqlStorageSession();
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