using System;
using FluentAssertions;
using Moq;
using NServiceBus.Persistence.Sql;
using NUnit.Framework;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.NServiceBus.SqlServer.ClientOutbox;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.SqlServer.UnitTests.ClientOutbox
{
    [TestFixture]
    public class ClientOutboxTransactionExtensionsTests : FluentTest<ClientOutboxTransactionExtensionsTestsFixture>
    {
        [Test]
        public void GetSqlSession_WhenGettingTheSqlSession_TheShouldReturnTheSqlSession()
        {
            Run(f => f.SetSqlSession(), f => f.GetSqlSession(), (f, r) => r.Should().Be(f.Session.Object));
        }

        [Test]
        public void GetSqlSession_WhenGettingTheSqlSessionAndItIsNotASqlStorageSession_ThenShouldThrowAnException()
        {
            Run(f => f.SetNonSqlSession(), f => f.GetSqlSession(), (f, a) => a.Should().Throw<Exception>()
                .WithMessage("Cannot access the SQL session"));
        }
    }

    public class ClientOutboxTransactionExtensionsTestsFixture
    {
        public Mock<IClientOutboxTransaction> Session { get; set; }
        public Mock<ISqlStorageSession> SqlSession { get; set; }

        public ISqlStorageSession GetSqlSession()
        {
            return Session.Object.GetSqlSession();
        }

        public ClientOutboxTransactionExtensionsTestsFixture SetSqlSession()
        {
            Session = new Mock<IClientOutboxTransaction>();
            SqlSession = Session.As<ISqlStorageSession>();

            return this;
        }

        public ClientOutboxTransactionExtensionsTestsFixture SetNonSqlSession()
        {
            Session = new Mock<IClientOutboxTransaction>();

            return this;
        }
    }
}