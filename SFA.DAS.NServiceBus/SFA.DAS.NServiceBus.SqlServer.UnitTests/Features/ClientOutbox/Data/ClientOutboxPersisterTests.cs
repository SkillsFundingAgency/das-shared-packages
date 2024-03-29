﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NServiceBus.Persistence;
using NServiceBus.Persistence.Sql;
using NServiceBus.Settings;
using NUnit.Framework;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Models;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.NServiceBus.SqlServer.Features.ClientOutbox.Data;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.SqlServer.UnitTests.Features.ClientOutbox.Data
{
    [TestFixture]
    public class ClientOutboxPersisterTests : FluentTest<ClientOutboxPersisterTestsFixture>
    {
        [Test]
        public Task BeginTransactionAsync_WhenBeginningATransaction_ThenShouldOpenTheConnection()
        {
            return TestAsync(f => f.BeginTransactionAsync(), f => f.Connection.Verify(c => c.OpenAsync(CancellationToken.None), Times.Once()));
        }

        [Test]
        public Task BeginTransactionAsync_WhenBeginningATransaction_ThenShouldBeginATransaction()
        {
            return TestAsync(f => f.BeginTransactionAsync(), f => f.Connection.Protected().Verify<DbTransaction>("BeginDbTransaction", Times.Once(), IsolationLevel.Unspecified));
        }

        [Test]
        public Task BeginTransactionAsync_WhenBeginningATransaction_ThenShouldReturnAClientOutboxTransaction()
        {
            return TestAsync(f => f.BeginTransactionAsync(), (_, r) => r.Should().NotBeNull().And.BeOfType<SqlClientOutboxTransaction>());
        }

        [Test]
        public Task StoreAsync_WhenStoringAClientOutboxMessage_ThenShouldStoreTheClientOutboxMessage()
        {
            return TestAsync(f => f.StoreAsync(), f =>
            {
                f.Connection.Protected().Verify("CreateDbCommand", Times.Once());
                f.Command.VerifySet(c => c.CommandText = ClientOutboxPersister.StoreCommandText);
                f.Command.VerifySet(c => c.Transaction = f.Transaction.Object);
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "MessageId" && p.Value as Guid? == f.ClientOutboxMessage.MessageId)));
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "EndpointName" && p.Value as string == f.EndpointName)));
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "CreatedAt" && p.Value as DateTime? == f.Now)));
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "Operations" && p.Value as string == f.EventsData)));
                f.Command.Verify(c => c.ExecuteNonQueryAsync(CancellationToken.None), Times.Once);
            });
        }

        [Test]
        public Task GetAsync_WhenGettingAClientOutboxMessage_TheShouldReturnTheClientOutboxMessage()
        {
            return TestAsync(f => f.SetupGetReaderWithRows(), f => f.GetAsync(), (f, r) =>
            {
                f.Connection.Protected().Verify("CreateDbCommand", Times.Once());
                f.Command.VerifySet(c => c.CommandText = ClientOutboxPersister.GetCommandText);
                f.Command.VerifySet(c => c.Transaction = f.Transaction.Object);
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "MessageId" && p.Value as Guid? == f.ClientOutboxMessage.MessageId)));
                r.Should().BeEquivalentTo(f.ClientOutboxMessage);
            });
        }

        [Test]
        public Task GetAsync_WhenGettingAClientOutboxMessageThatDoesNotExist_ThenShouldThrowAnException()
        {
            return TestExceptionAsync(f => f.SetupGetReaderWithNoRows(), f => f.GetAsync(), (f, a) => a.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Client outbox data not found where MessageId = '{f.ClientOutboxMessage.MessageId}'"));
        }

        [Test]
        public Task GetAwaitingDispatchAsync_WhenGettingClientOutboxMessagesAwaitingDispatch_TheShouldReturnClientOutboxMessagesAwaitingDispatch()
        {
            return TestAsync(f => f.SetupGetAwaitingDispatchReader(), f => f.GetAwaitingDispatchAsync(), (f, r) =>
            {
                f.Connection.Protected().Verify("CreateDbCommand", Times.Once());
                f.Command.VerifySet(c => c.CommandText = ClientOutboxPersister.GetAwaitingDispatchCommandText);
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "CreatedAt" && p.Value as DateTime? == f.Now.AddMinutes(-10))));
                r.Should().BeEquivalentTo(f.OutboxMessages);
                f.Connection.As<IDisposable>().Verify(c => c.Dispose(), Times.Once());
            });
        }

        [Test]
        public Task SetAsDispatchedAsync_WhenSettingAClientOutboxMessageAsDispatched_ThenShouldSetTheClientOutboxMessageAsDispatched()
        {
            return TestAsync(f => f.SetAsDispatchedAsync(), f =>
            {
                f.Connection.Protected().Verify("CreateDbCommand", Times.Once());
                f.Command.VerifySet(c => c.CommandText = ClientOutboxPersister.SetAsDispatchedCommandText);
                f.Command.VerifySet(c => c.Transaction = f.Transaction.Object);
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "MessageId" && p.Value as Guid? == f.ClientOutboxMessage.MessageId)));
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "DispatchedAt" && p.Value as DateTime? == f.Now)));
                f.Command.Verify(c => c.ExecuteNonQueryAsync(CancellationToken.None), Times.Once);
            });
        }
        
        [TestCase(0, false, 1)]
        [TestCase(10000, false, 2)]
        [TestCase(20000, false, 3)]
        [TestCase(20000, true, 0)]
        public Task RemoveEntriesOlderThanAsync_WhenRemovingOldClientOutboxMessages_ThenShouldRemoveOldClientOutboxMessagesInBatches(int oldMessageCount, bool isCancellationRequested, int expectedBatchCount)
        {
            return TestAsync(f => f.SetOldClientOutboxMessageCount(oldMessageCount).SetCancellationRequested(isCancellationRequested), f => f.RemoveEntriesOlderThanAsync(), f =>
            {
                f.Connection.Protected().Verify("CreateDbCommand", Times.Exactly(expectedBatchCount));
                f.Command.VerifySet(c => c.CommandText = ClientOutboxPersister.RemoveEntriesOlderThanCommandText, Times.Exactly(expectedBatchCount));
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "BatchSize" && p.Value as int? == ClientOutboxPersister.CleanupBatchSize)), Times.Exactly(expectedBatchCount));
                f.Parameters.Verify(ps => ps.Add(It.Is<DbParameter>(p => p.ParameterName == "DispatchedBefore" && p.Value as DateTime? == f.Now)), Times.Exactly(expectedBatchCount));
                f.Connection.As<IDisposable>().Verify(c => c.Dispose(), Times.Once());
            });
        }
    }

    public class ClientOutboxPersisterTestsFixture
    {
        public DateTime Now { get; set; }
        public IClientOutboxStorage ClientOutboxStorage { get; set; }
        public Mock<IDateTimeService> DateTimeService { get; set; }
        public Mock<ReadOnlySettings> Settings { get; set; }
        public Mock<DbConnection> Connection { get; set; }
        public Mock<DbTransaction> Transaction { get; set; }
        public Mock<DbCommand> Command { get; set; }
        public Mock<DbParameterCollection> Parameters { get; set; }
        public IClientOutboxTransaction ClientOutboxTransaction { get; set; }
        public string EndpointName { get; set; }
        public List<object> Events { get; set; }
        public string EventsData { get; set; }
        public ClientOutboxMessage ClientOutboxMessage { get; set; }
        public Mock<SynchronizedStorageSession> SynchronizedStorageSession { get; set; }
        public Mock<ISqlStorageSession> SqlSession { get; set; }
        public List<IClientOutboxMessageAwaitingDispatch> OutboxMessages { get; set; }
        public Mock<DbDataReader> DataReader { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public ClientOutboxPersisterTestsFixture()
        {
            Now = DateTime.UtcNow;
            DateTimeService = new Mock<IDateTimeService>();
            Settings = new Mock<ReadOnlySettings>();
            Connection = new Mock<DbConnection>();
            Connection.As<IDisposable>().Setup(_ => _.Dispose());
            Transaction = new Mock<DbTransaction> { CallBase = true };
            Transaction.As<IDisposable>().Setup(_ => _.Dispose());
            Command = new Mock<DbCommand>();
            Parameters = new Mock<DbParameterCollection>();
            ClientOutboxTransaction = new SqlClientOutboxTransaction(Connection.Object, Transaction.Object);
            EndpointName = "SFA.DAS.NServiceBus";

            Events = new List<object>
            {
                new FooEvent(Now.AddDays(-1)),
                new BarEvent(Now)
            };

            EventsData = JsonConvert.SerializeObject(Events, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            ClientOutboxMessage = new ClientOutboxMessage(GuidComb.NewGuidComb(), EndpointName, Events);
            SynchronizedStorageSession = new Mock<SynchronizedStorageSession>();
            SqlSession = SynchronizedStorageSession.As<ISqlStorageSession>();
            OutboxMessages = new List<IClientOutboxMessageAwaitingDispatch>();
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;
            
            DateTimeService.Setup(d => d.UtcNow).Returns(Now);
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
            Settings.Setup(s => s.Get<Func<DbConnection>>("SqlPersistence.ConnectionBuilder")).Returns(() => Connection.Object);
            SqlSession.Setup(s => s.Connection).Returns(Connection.Object);
            SqlSession.Setup(s => s.Transaction).Returns(Transaction.Object);

            ClientOutboxStorage = new ClientOutboxPersister(DateTimeService.Object, Settings.Object);
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
            return ClientOutboxStorage.GetAsync(ClientOutboxMessage.MessageId, SynchronizedStorageSession.Object);
        }

        public Task<IEnumerable<IClientOutboxMessageAwaitingDispatch>> GetAwaitingDispatchAsync()
        {
            return ClientOutboxStorage.GetAwaitingDispatchAsync();
        }

        public Task SetAsDispatchedAsync()
        {
            return ClientOutboxStorage.SetAsDispatchedAsync(ClientOutboxMessage.MessageId, SynchronizedStorageSession.Object);
        }

        public Task RemoveEntriesOlderThanAsync()
        {
            return ClientOutboxStorage.RemoveEntriesOlderThanAsync(Now, CancellationToken);
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

        public ClientOutboxPersisterTestsFixture SetOldClientOutboxMessageCount(int count)
        {
            Command.Setup(c => c.ExecuteNonQueryAsync(CancellationToken)).ReturnsAsync(() =>
            {
                var rowsAffected = ClientOutboxPersister.CleanupBatchSize > count ? count : ClientOutboxPersister.CleanupBatchSize;
                
                count -= ClientOutboxPersister.CleanupBatchSize;

                return rowsAffected;
            });
            
            return this;
        }

        public ClientOutboxPersisterTestsFixture SetCancellationRequested(bool isCancellationRequested)
        {
            if (isCancellationRequested)
            {
                CancellationTokenSource.Cancel();
            }
            
            return this;
        }
    }
}