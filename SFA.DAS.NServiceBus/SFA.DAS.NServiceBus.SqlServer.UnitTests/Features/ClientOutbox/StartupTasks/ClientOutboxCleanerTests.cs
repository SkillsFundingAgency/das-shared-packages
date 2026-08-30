using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
using NServiceBus.Settings;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Models;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.NServiceBus.SqlServer.Features.ClientOutbox.StartupTasks;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.SqlServer.UnitTests.Features.ClientOutbox.StartupTasks
{
    [TestFixture]
    public class ClientOutboxCleanerTests : FluentTest<ClientOutboxCleanerTestsFixture>
    {
        [Test]
        public Task OnStart_WhenClientOutboxMessagesAreAwaitingDispatch_ThenShouldSendProcessClientOutboxMessageCommands()
        {
            return TestAsync(f => f.SetClientOutboxMessagesAwaitingDispatch(), f => f.OnStart(new CancellationToken()), f =>
            {
                f.MessageSession.SentMessages.Should().HaveCount(f.ClientOutboxMessages.Count + f.ClientOutboxMessageV2s.Count);
                
                f.MessageSession.SentMessages
                    .Where(m => m.Message is ProcessClientOutboxMessageCommand)
                    .Select(m => new { MessageId = Guid.Parse(m.Options.GetMessageId()), EndpointName = m.Options.GetDestination() })
                    .Should().BeEquivalentTo(f.ClientOutboxMessages);
                
                f.MessageSession.SentMessages
                    .Where(m => m.Message is ProcessClientOutboxMessageCommandV2)
                    .Select(m => new { MessageId = Guid.Parse(m.Options.GetMessageId()), EndpointName = m.Options.GetDestination() })
                    .Should().BeEquivalentTo(f.ClientOutboxMessageV2s);
            });
        }
        
        [Test]
        public Task OnStart_WhenClientOutboxMessagesAreAwaitingDispatch_ThenShouldRemoveOldEntries()
        {
            return TestAsync(f => f.OnStart(new CancellationToken()), f =>
            {
                f.ClientOutboxStorage.Verify(s => s.RemoveEntriesOlderThanAsync(f.Now.Subtract(f.MaxAge), f.CancellationToken), Times.Once);
                f.ClientOutboxStorageV2.Verify(s => s.RemoveEntriesOlderThanAsync(f.Now.Subtract(f.MaxAge), f.CancellationToken), Times.Once);
            });
        }
        
        [Test]
        public Task OnStart_WhenNoClientOutboxMessagesAreAwaitingDispatch_ThenShouldNotSendProcessClientOutboxMessageCommands()
        {
            return TestAsync(f => f.OnStart(new CancellationToken()), f => f.MessageSession.SentMessages.Should().BeEmpty());
        }
        
        [Test]
        public Task OnStart_WhenConsecutiveExceptionsAreThrown_ThenShouldRaiseCriticalError()
        {
            return TestAsync(f => f.OnStartWithConsecutiveExceptions(new CancellationToken()), f => f.CriticalError.Verify(e => e.Raise(It.IsAny<string>(), f.Exception, It.IsAny<CancellationToken>()), Times.Once));
        }
        
        [Test]
        public Task OnStart_WhenNonConsecutiveExceptionsAreThrown_ThenShouldRaiseCriticalError()
        {
            return TestAsync(f => f.OnStartWithNonConsecutiveExceptions(new CancellationToken()), f => f.CriticalError.Verify(e => e.Raise(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<CancellationToken>()), Times.Never));
        }
        
        [Test]
        public Task OnStop_WhenStopping_ThenShouldStopTimer()
        {
            return TestAsync(f => f.OnStop(new CancellationToken()), f => f.TimerService.Verify(t => t.Stop(), Times.Once));
        }
    }

    public class ClientOutboxCleanerTestsFixture
    {
        public DateTime Now { get; set; }
        public TestableMessageSession MessageSession { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public Mock<ITimerService> TimerService { get; set; }
        public List<IClientOutboxMessageAwaitingDispatch> ClientOutboxMessages { get; set; }
        public List<IClientOutboxMessageAwaitingDispatch> ClientOutboxMessageV2s { get; set; }
        public TestableClientOutboxCleaner Cleaner { get; set; }
        public Mock<IClientOutboxStorage> ClientOutboxStorage { get; set; }
        public Mock<IClientOutboxStorageV2> ClientOutboxStorageV2 { get; set; }
        public Mock<IReadOnlySettings> Settings { get; set; }
        public Mock<CriticalError> CriticalError { get; set; }
        public Func<DateTime, CancellationToken, Task> TimerServiceSuccessCallback { get; set; }
        public Action<Exception> TimerServiceErrorCallback { get; set; }
        public TimeSpan Frequency { get; set; }
        public TimeSpan MaxAge { get; set; }
        public Exception Exception { get; set; }

        public ClientOutboxCleanerTestsFixture()
        {
            Now = DateTime.UtcNow;
            MessageSession = new TestableMessageSession();
            CancellationToken = CancellationToken.None;
            TimerService = new Mock<ITimerService>();
            ClientOutboxMessages = new List<IClientOutboxMessageAwaitingDispatch>();
            ClientOutboxMessageV2s = new List<IClientOutboxMessageAwaitingDispatch>();
            ClientOutboxStorage = new Mock<IClientOutboxStorage>();
            ClientOutboxStorageV2 = new Mock<IClientOutboxStorageV2>();
            Settings = new Mock<IReadOnlySettings>();
            CriticalError = new Mock<CriticalError>(null);
            Frequency = TimeSpan.Zero;
            MaxAge = TimeSpan.FromDays(14);
            Exception = new Exception();
            
            TimerService.Setup(t => t.Start(It.IsAny<Func<DateTime, CancellationToken, Task>>(), It.IsAny<Action<Exception>>(), Frequency))
                .Callback<Func<DateTime, CancellationToken, Task>, Action<Exception>, TimeSpan>((s, e, f) =>
                {
                    TimerServiceSuccessCallback = s;
                    TimerServiceErrorCallback = e;
                });
            
            ClientOutboxStorage.Setup(o => o.GetAwaitingDispatchAsync()).ReturnsAsync(ClientOutboxMessages);
            ClientOutboxStorageV2.Setup(o => o.GetAwaitingDispatchAsync()).ReturnsAsync(ClientOutboxMessageV2s);
            Settings.Setup(s => s.GetOrDefault<TimeSpan?>("Persistence.Sql.Outbox.FrequencyToRunDeduplicationDataCleanup")).Returns(Frequency);
            Settings.Setup(s => s.GetOrDefault<TimeSpan?>("Persistence.Sql.Outbox.TimeToKeepDeduplicationData")).Returns(MaxAge);
            
            Cleaner = new TestableClientOutboxCleaner(TimerService.Object, ClientOutboxStorage.Object, ClientOutboxStorageV2.Object, Settings.Object, CriticalError.Object);
        }

        public async Task OnStart(CancellationToken cancellationToken)
        {
            await Cleaner.Start(MessageSession, cancellationToken);
            await TimerServiceSuccessCallback(Now, CancellationToken);
        }

        public async Task OnStartWithConsecutiveExceptions(CancellationToken cancellationToken)
        {
            await Cleaner.Start(MessageSession, cancellationToken);

            for (var i = 0; i < 10; i++)
            {
                TimerServiceErrorCallback(Exception);
            }
        }

        public async Task OnStartWithNonConsecutiveExceptions(CancellationToken cancellationToken)
        {
            await Cleaner.Start(MessageSession, cancellationToken);

            for (var i = 0; i < 100; i++)
            {
                if (i % 9 == 0)
                {
                    await TimerServiceSuccessCallback(Now, CancellationToken);
                }
                else
                {
                    TimerServiceErrorCallback(Exception);
                }
            }
        }

        public async Task OnStop(CancellationToken cancellationToken)
        {
            await Cleaner.Stop(MessageSession, cancellationToken);
        }

        public ClientOutboxCleanerTestsFixture SetClientOutboxMessagesAwaitingDispatch()
        {
            ClientOutboxMessages.Add(new ClientOutboxMessage(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.Foo"));
            ClientOutboxMessages.Add(new ClientOutboxMessage(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.Bar"));
            ClientOutboxMessageV2s.Add(new ClientOutboxMessageV2(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.FooV2"));
            ClientOutboxMessageV2s.Add(new ClientOutboxMessageV2(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.BarV2"));

            return this;
        }

        public class TestableClientOutboxCleaner : ClientOutboxCleaner
        {
            public TestableClientOutboxCleaner(ITimerService timerService, IClientOutboxStorage clientOutboxStorage, IClientOutboxStorageV2 clientOutboxStorageV2, IReadOnlySettings settings, CriticalError criticalError)
                : base(timerService, clientOutboxStorage, clientOutboxStorageV2, settings, criticalError)
            {
            }

            public Task Start(IMessageSession messageSession, CancellationToken cancellationToken)
            {
                return OnStart(messageSession, cancellationToken);
            }

            public Task Stop(IMessageSession messageSession, CancellationToken cancellationToken)
            {
                return OnStop(messageSession, cancellationToken);
            }
        }
    }
}