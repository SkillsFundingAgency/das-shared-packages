using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
using NServiceBus.Testing;
using NUnit.Framework;

namespace SFA.DAS.NServiceBus.UnitTests
{
    [TestFixture]
    public class ProcessOutboxMessagesJobTests : FluentTest<ProcessOutboxMessagesJobTestsFixture>
    {
        [Test]
        public Task RunAsync_WhenRunningAndOutboxMessagesNeedProcessing_ThenShouldSendProcessOutboxMessageCommands()
        {
            return RunAsync(f => f.SetOutboxMessageIds(), f => f.RunAsync(), f => f.Session.SentMessages.Should().HaveCount(2).And.Match<IEnumerable<SentMessage<object>>>(m => 
                m.ElementAt(0).Message is ProcessOutboxMessageCommand && m.ElementAt(0).Options.GetMessageId() == f.OutboxMessageIds[0].ToString() &&
                m.ElementAt(1).Message is ProcessOutboxMessageCommand && m.ElementAt(1).Options.GetMessageId() == f.OutboxMessageIds[1].ToString()));
        }

        [Test]
        public Task RunAsync_WhenRunningAndNoOutboxMessagesNeedProcessing_ThenShouldNotSendProcessOutboxMessageCommands()
        {
            return RunAsync(f => f.RunAsync(), f => f.Session.SentMessages.Should().BeEmpty());
        }
    }

    public class ProcessOutboxMessagesJobTestsFixture : FluentTestFixture
    {
        public TestableMessageSession Session { get; set; }
        public List<Guid> OutboxMessageIds { get; set; }
        public IProcessOutboxMessagesJob Job { get; set; }
        public Mock<IOutbox> Outbox { get; set; }

        public ProcessOutboxMessagesJobTestsFixture()
        {
            Session = new TestableMessageSession();
            OutboxMessageIds = new List<Guid>();
            Outbox = new Mock<IOutbox>();

            Outbox.Setup(o => o.GetIdsToProcess()).ReturnsAsync(OutboxMessageIds);

            Job = new ProcessOutboxMessagesJob(Session, Outbox.Object);
        }

        public Task RunAsync()
        {
            return Job.RunAsync();
        }

        public ProcessOutboxMessagesJobTestsFixture SetOutboxMessageIds()
        {
            OutboxMessageIds.Add(GuidComb.NewGuidComb());
            OutboxMessageIds.Add(GuidComb.NewGuidComb());

            return this;
        }
    }
}