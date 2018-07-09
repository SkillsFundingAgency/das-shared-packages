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
using SFA.DAS.NServiceBus.MsSqlServer;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.MsSqlServer
{
    [TestFixture]
    public class OutboxTests : FluentTest<OutboxTestsFixture>
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
            return RunAsync(f => f.BeginTransactionAsync(), (f, r) => r.Should().NotBeNull().And.BeOfType<OutboxTransaction>());
        }

        [Test]
        public Task StoreAsync_WhenStoringAnOutboxMessage_ThenShouldStoreTheOutboxMessage()
        {
            return RunAsync(f => f.StoreAsync(), f =>
            {
                f.Connection.Protected().Verify("CreateDbCommand", Times.Once());
                f.Command.VerifySet(c => c.CommandText = Outbox.StoreCommandText);
                f.Command.VerifySet(c => c.Transaction = f.Transaction.Object);
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "MessageId" && p.Value as Guid? == f.OutboxMessage.MessageId)));
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "EndpointName" && p.Value as string == f.EndpointName)));
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "Created" && p.Value as DateTime? >= f.Now)));
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
                f.Command.VerifySet(c => c.CommandText = Outbox.GetCommandText);
                f.Command.VerifySet(c => c.Transaction = f.Transaction.Object);
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "MessageId" && p.Value as Guid? == f.OutboxMessage.MessageId)));
                r.ShouldBeEquivalentTo(f.OutboxMessage);
            });
        }

        [Test]
        public Task GetAsync_WhenGettingAnOutboxMessageThatDoesNotExist_ThenShouldThrowAnException()
        {
            return RunAsync(f => f.SetupGetReaderWithNoRows(), f => f.GetAsync(), (f, a) => a.ShouldThrow<KeyNotFoundException>()
                .WithMessage($"Client outbox data not found where MessageId = '{f.OutboxMessage.MessageId}'"));
        }

        [Test]
        public Task GetAwaitingDispatchAsync_WhenGettingOutboxMessagesAwaitingDispatch_TheShouldReturnOutboxMessagesAwaitingDispatch()
        {
            return RunAsync(f => f.SetupGetAwaitingDispatchReader(), f => f.GetAwaitingDispatchAsync(), (f, r) =>
            {
                f.Connection.Protected().Verify("CreateDbCommand", Times.Once());
                f.Connection.Verify(c => c.Close(), Times.Once);
                f.Command.VerifySet(c => c.CommandText = Outbox.GetAwaitingDispatchCommandText);
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "Created" && p.Value as DateTime? <= f.Now)));
                r.ShouldAllBeEquivalentTo(f.OutboxMessages);
            });
        }

        [Test]
        public Task SetAsDispatchedAsync_WhenSettingAnOutboxMessageAsDispatched_ThenShouldSetTheOutboxMessageAsDispatched()
        {
            return RunAsync(f => f.SetAsDispatchedAsync(), f =>
            {
                f.Connection.Protected().Verify("CreateDbCommand", Times.Once());
                f.Command.VerifySet(c => c.CommandText = Outbox.SetAsDispatchedCommandText);
                f.Command.VerifySet(c => c.Transaction = f.Transaction.Object);
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "MessageId" && p.Value as Guid? == f.OutboxMessage.MessageId)));
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "DispatchedAt" && p.Value as DateTime? >= f.Now)));
                f.Command.Verify(c => c.ExecuteNonQueryAsync(CancellationToken.None), Times.Once);
            });
        }
    }

    public class OutboxTestsFixture : FluentTestFixture
    {
        public DateTime Now { get; set; }
        public IOutbox Outbox { get; set; }
        public Mock<DbConnection> Connection { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public Mock<ReadOnlySettings> Settings { get; set; }
        public Mock<DbTransaction> Transaction { get; set; }
        public Mock<DbCommand> Command { get; set; }
        public Mock<DbParameterCollection> Parameters { get; set; }
        public IOutboxTransaction OutboxTransaction { get; set; }
        public string EndpointName { get; set; }
        public List<Event> Events { get; set; }
        public string EventsData { get; set; }
        public OutboxMessage OutboxMessage { get; set; }
        public List<IOutboxMessageAwaitingDispatch> OutboxMessages { get; set; }
        public Mock<DbDataReader> DataReader { get; set; }

        public OutboxTestsFixture()
        {
            Now = DateTime.UtcNow;
            Connection = new Mock<DbConnection>();
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            Settings = new Mock<ReadOnlySettings>();
            Transaction = new Mock<DbTransaction> { CallBase = true };
            Command = new Mock<DbCommand>();
            Parameters = new Mock<DbParameterCollection>();
            OutboxTransaction = new OutboxTransaction(Transaction.Object);
            EndpointName = "SFA.DAS.NServiceBus";

            Events = new List<Event>
            {
                new FooEvent { Created = Now.AddDays(-1) },
                new BarEvent { Created = Now }
            };

            EventsData = JsonConvert.SerializeObject(Events, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            OutboxMessage = new OutboxMessage(GuidComb.NewGuidComb(), EndpointName, Events);
            OutboxMessages = new List<IOutboxMessageAwaitingDispatch>();

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

            Outbox = new Outbox(Connection.Object, UnitOfWorkContext.Object);
        }

        public Task<IOutboxTransaction> BeginTransactionAsync()
        {
            return Outbox.BeginTransactionAsync();
        }

        public Task StoreAsync()
        {
            return Outbox.StoreAsync(OutboxMessage, OutboxTransaction);
        }

        public Task<OutboxMessage> GetAsync()
        {
            return Outbox.GetAsync(OutboxMessage.MessageId);
        }

        public Task<IEnumerable<IOutboxMessageAwaitingDispatch>> GetAwaitingDispatchAsync()
        {
            return Outbox.GetAwaitingDispatchAsync();
        }

        public Task SetAsDispatchedAsync()
        {
            return Outbox.SetAsDispatchedAsync(OutboxMessage.MessageId);
        }

        public OutboxTestsFixture SetupGetReaderWithRows()
        {
            DataReader = new Mock<DbDataReader>();
            
            DataReader.Setup(r => r.ReadAsync(CancellationToken.None)).ReturnsAsync(true);
            DataReader.Setup(r => r.GetString(0)).Returns(EndpointName);
            DataReader.Setup(r => r.GetString(1)).Returns(EventsData);
            Command.Protected().Setup<Task<DbDataReader>>("ExecuteDbDataReaderAsync", CommandBehavior.SingleRow, CancellationToken.None).ReturnsAsync(DataReader.Object);

            return this;
        }

        public OutboxTestsFixture SetupGetReaderWithNoRows()
        {
            DataReader = new Mock<DbDataReader>();

            DataReader.Setup(r => r.ReadAsync(CancellationToken.None)).ReturnsAsync(false);
            Command.Protected().Setup<Task<DbDataReader>>("ExecuteDbDataReaderAsync", CommandBehavior.SingleRow, CancellationToken.None).ReturnsAsync(DataReader.Object);

            return this;
        }

        public OutboxTestsFixture SetupGetAwaitingDispatchReader()
        {
            OutboxMessages = new List<IOutboxMessageAwaitingDispatch>
            {
                new OutboxMessage(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.Foo"),
                new OutboxMessage(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.Foo")
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