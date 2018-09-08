using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NServiceBus.Settings;
using NUnit.Framework;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.NServiceBus.SqlServer.ClientOutbox;
using SFA.DAS.NServiceBus.UnitOfWork;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.SqlServer.ClientOutbox
{
    [TestFixture]
    public class ClientOutboxPersisterTests : FluentTest<ClientOutboxPersisterTestsFixture>
    {
        [Test]
        public Task BeginTransactionAsync_WhenBeginningATransaction_ThenShouldOpenTheConnection()
        {
            return RunAsync(f => f.BeginTransactionAsync(), f => f.Connection.Verify(c => c.OpenAsync(CancellationToken.None), Times.Once()));
        }

        [Test]
        public Task BeginTransactionAsync_WhenBeginningATransaction_ThenShouldBeginATransaction()
        {
            return RunAsync(f => f.BeginTransactionAsync(), f => f.Connection.Protected().Verify<DbTransaction>("BeginDbTransaction", Times.Once(), IsolationLevel.Unspecified));
        }

        [Test]
        public Task BeginTransactionAsync_WhenBeginningATransaction_ThenShouldSetUnitOfWorkContextDbConnection()
        {
            return RunAsync(f => f.BeginTransactionAsync(), f => f.UnitOfWorkContext.Verify(c => c.Set(f.Connection.Object), Times.Once()));
        }

        [Test]
        public Task BeginTransactionAsync_WhenBeginningATransaction_ThenShouldSetUnitOfWorkContextDbTransaction()
        {
            return RunAsync(f => f.BeginTransactionAsync(), f => f.UnitOfWorkContext.Verify(c => c.Set(f.Transaction.Object), Times.Once()));
        }

        [Test]
        public Task BeginTransactionAsync_WhenBeginningATransaction_ThenShouldReturnAnOutboxTransaction()
        {
            return RunAsync(f => f.BeginTransactionAsync(), (f, r) => r.Should().NotBeNull().And.BeOfType<SqlClientOutboxTransaction>());
        }

        [Test]
        public Task StoreAsync_WhenStoringAnOutboxMessage_ThenShouldStoreTheOutboxMessage()
        {
            return RunAsync(f => f.StoreAsync(), f =>
            {
                f.Connection.Protected().Verify("CreateDbCommand", Times.Once());
                f.Command.VerifySet(c => c.CommandText = ClientOutboxPersister.StoreCommandText);
                f.Command.VerifySet(c => c.Transaction = f.Transaction.Object);
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "MessageId" && p.Value as Guid? == f.ClientOutboxMessage.MessageId)));
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "EndpointName" && p.Value as string == f.EndpointName)));
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "CreatedAt" && p.Value as DateTime? >= f.Now)));
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "Operations" && p.Value as string == f.EventsData)));
                f.Command.Verify(c => c.ExecuteNonQueryAsync(CancellationToken.None), Times.Once);
            });
        }

        [Test]
        public Task GetAsync_WhenGettingAnOutboxMessage_TheShouldReturnTheOutboxMessage()
        {
            return RunAsync(f => f.SetupGetReaderWithRows(), f => f.GetAsync(), (f, r) =>
            {
                f.Connection.Protected().Verify("CreateDbCommand", Times.Once());
                f.Command.VerifySet(c => c.CommandText = ClientOutboxPersister.GetCommandText);
                f.Command.VerifySet(c => c.Transaction = f.Transaction.Object);
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "MessageId" && p.Value as Guid? == f.ClientOutboxMessage.MessageId)));
                r.ShouldBeEquivalentTo(f.ClientOutboxMessage);
            });
        }

        [Test]
        public Task GetAsync_WhenGettingAnOutboxMessageThatDoesNotExist_ThenShouldThrowAnException()
        {
            return RunAsync(f => f.SetupGetReaderWithNoRows(), f => f.GetAsync(), (f, a) => a.ShouldThrow<KeyNotFoundException>()
                .WithMessage($"Client outbox data not found where MessageId = '{f.ClientOutboxMessage.MessageId}'"));
        }

        [Test]
        public Task GetAwaitingDispatchAsync_WhenGettingOutboxMessagesAwaitingDispatch_TheShouldReturnOutboxMessagesAwaitingDispatch()
        {
            return RunAsync(f => f.SetupGetAwaitingDispatchReader(), f => f.GetAwaitingDispatchAsync(), (f, r) =>
            {
                f.Connection.Protected().Verify("CreateDbCommand", Times.Once());
                f.Connection.Verify(c => c.Close(), Times.Once);
                f.Command.VerifySet(c => c.CommandText = ClientOutboxPersister.GetAwaitingDispatchCommandText);
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "CreatedAt" && p.Value as DateTime? <= f.Now)));
                r.ShouldAllBeEquivalentTo(f.OutboxMessages);
            });
        }

        [Test]
        public Task SetAsDispatchedAsync_WhenSettingAnOutboxMessageAsDispatched_ThenShouldSetTheOutboxMessageAsDispatched()
        {
            return RunAsync(f => f.SetAsDispatchedAsync(), f =>
            {
                f.Connection.Protected().Verify("CreateDbCommand", Times.Once());
                f.Command.VerifySet(c => c.CommandText = ClientOutboxPersister.SetAsDispatchedCommandText);
                f.Command.VerifySet(c => c.Transaction = f.Transaction.Object);
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "MessageId" && p.Value as Guid? == f.ClientOutboxMessage.MessageId)));
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "DispatchedAt" && p.Value as DateTime? >= f.Now)));
                f.Command.Verify(c => c.ExecuteNonQueryAsync(CancellationToken.None), Times.Once);
            });
        }
    }

    public class ClientOutboxPersisterTestsFixture : FluentTestFixture
    {
        public DateTime Now { get; set; }
        public IClientOutboxStorage ClientOutboxStorage { get; set; }
        public Mock<DbConnection> Connection { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public Mock<ReadOnlySettings> Settings { get; set; }
        public Mock<DbTransaction> Transaction { get; set; }
        public Mock<DbCommand> Command { get; set; }
        public Mock<DbParameterCollection> Parameters { get; set; }
        public IClientOutboxTransaction ClientOutboxTransaction { get; set; }
        public string EndpointName { get; set; }
        public List<Event> Events { get; set; }
        public string EventsData { get; set; }
        public ClientOutboxMessage ClientOutboxMessage { get; set; }
        public List<IClientOutboxMessageAwaitingDispatch> OutboxMessages { get; set; }
        public Mock<DbDataReader> DataReader { get; set; }

        public ClientOutboxPersisterTestsFixture()
        {
            Now = DateTime.UtcNow;
            Connection = new Mock<DbConnection>();
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            Settings = new Mock<ReadOnlySettings>();
            Transaction = new Mock<DbTransaction> { CallBase = true };
            Command = new Mock<DbCommand>();
            Parameters = new Mock<DbParameterCollection>();
            ClientOutboxTransaction = new SqlClientOutboxTransaction(Transaction.Object);
            EndpointName = "SFA.DAS.NServiceBus";

            Events = new List<Event>
            {
                new FooEvent { Created = Now.AddDays(-1) },
                new BarEvent { Created = Now }
            };

            EventsData = JsonConvert.SerializeObject(Events, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            ClientOutboxMessage = new ClientOutboxMessage(GuidComb.NewGuidComb(), EndpointName, Events);
            OutboxMessages = new List<IClientOutboxMessageAwaitingDispatch>();

            Parameters.Setup(p => p.Add(It.IsAny<DbParameter>()));
            Command.SetupSet(c => c.CommandText = It.IsAny<string>());
            Command.SetupSet(c => c.Transaction = It.IsAny<DbTransaction>());

            Command.Protected().Setup<DbParameter>("CreateDbParameter").Returns(() =>
            {
                var parameter = new Mock<DbParameter> { CallBase = true };

                parameter.SetupProperty(p => p.ParameterName);
                parameter.SetupProperty(p => p.Value);

                return parameter.Object;
            });

            Command.Protected().Setup<DbParameterCollection>("DbParameterCollection").Returns(Parameters.Object);
            Connection.Protected().Setup<DbTransaction>("BeginDbTransaction", IsolationLevel.Unspecified).Returns(Transaction.Object);
            Connection.Protected().Setup<DbCommand>("CreateDbCommand").Returns(Command.Object);
            UnitOfWorkContext.Setup(c => c.Get<DbConnection>()).Returns(Connection.Object);
            UnitOfWorkContext.Setup(c => c.Get<DbTransaction>()).Returns(Transaction.Object);
            Settings.Setup(s => s.Get<string>("NServiceBus.Routing.EndpointName")).Returns(EndpointName);

            ClientOutboxStorage = new ClientOutboxPersister(Connection.Object, UnitOfWorkContext.Object);
        }

        public Task<IClientOutboxTransaction> BeginTransactionAsync()
        {
            return ClientOutboxStorage.BeginTransactionAsync();
        }

        public Task StoreAsync()
        {
            return ClientOutboxStorage.StoreAsync(ClientOutboxMessage, ClientOutboxTransaction);
        }

        public Task<ClientOutboxMessage> GetAsync()
        {
            return ClientOutboxStorage.GetAsync(ClientOutboxMessage.MessageId);
        }

        public Task<IEnumerable<IClientOutboxMessageAwaitingDispatch>> GetAwaitingDispatchAsync()
        {
            return ClientOutboxStorage.GetAwaitingDispatchAsync();
        }

        public Task SetAsDispatchedAsync()
        {
            return ClientOutboxStorage.SetAsDispatchedAsync(ClientOutboxMessage.MessageId);
        }

        public ClientOutboxPersisterTestsFixture SetupGetReaderWithRows()
        {
            DataReader = new Mock<DbDataReader>();

            DataReader.Setup(r => r.ReadAsync(CancellationToken.None)).ReturnsAsync(true);
            DataReader.Setup(r => r.GetString(0)).Returns(EndpointName);
            DataReader.Setup(r => r.GetString(1)).Returns(EventsData);
            Command.Protected().Setup<Task<DbDataReader>>("ExecuteDbDataReaderAsync", CommandBehavior.SingleRow, CancellationToken.None).ReturnsAsync(DataReader.Object);

            return this;
        }

        public ClientOutboxPersisterTestsFixture SetupGetReaderWithNoRows()
        {
            DataReader = new Mock<DbDataReader>();

            DataReader.Setup(r => r.ReadAsync(CancellationToken.None)).ReturnsAsync(false);
            Command.Protected().Setup<Task<DbDataReader>>("ExecuteDbDataReaderAsync", CommandBehavior.SingleRow, CancellationToken.None).ReturnsAsync(DataReader.Object);

            return this;
        }

        public ClientOutboxPersisterTestsFixture SetupGetAwaitingDispatchReader()
        {
            OutboxMessages = new List<IClientOutboxMessageAwaitingDispatch>
            {
                new ClientOutboxMessage(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.Foo"),
                new ClientOutboxMessage(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.Foo")
            };

            DataReader = new Mock<DbDataReader>();

            DataReader.SetupSequence(r => r.ReadAsync(CancellationToken.None)).ReturnsAsync(true).ReturnsAsync(true).ReturnsAsync(false);
            DataReader.SetupSequence(r => r.GetGuid(0)).Returns(OutboxMessages[0].MessageId).Returns(OutboxMessages[1].MessageId);
            DataReader.SetupSequence(r => r.GetString(1)).Returns(OutboxMessages[0].EndpointName).Returns(OutboxMessages[1].EndpointName);
            Command.Protected().Setup<Task<DbDataReader>>("ExecuteDbDataReaderAsync", CommandBehavior.Default, CancellationToken.None).ReturnsAsync(DataReader.Object);

            return this;
        }
    }
}