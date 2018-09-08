using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.NServiceBus.ClientOutbox;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.ClientOutbox
{
    [TestFixture]
    public class ProcessClientOutboxMessagesJobTests : FluentTest<ProcessOutboxMessagesJobTestsFixture>
    {
        [Test]
        public Task RunAsync_WhenRunningAndOutboxMessagesAreAwaitingDispatch_ThenShouldSendProcessClientOutboxMessageCommands()
        {
            return RunAsync(f => f.SetOutboxMessagesAwaitingDispatch(), f => f.RunAsync(), f =>
            {
                f.Session.SentMessages.Select(m => m.Message).Should().HaveCount(2).And.ContainItemsAssignableTo<ProcessClientOutboxMessageCommand>();
                f.Session.SentMessages.Select(m => new { MessageId = Guid.Parse(m.Options.GetMessageId()), EndpointName = m.Options.GetDestination() }).ShouldAllBeEquivalentTo(f.ClientOutboxMessages);
            });
        }

        [Test]
        public Task RunAsync_WhenRunningAndNoOutboxMessagesAreAwaitingDispatch_ThenShouldNotSendProcessClientOutboxMessageCommands()
        {
            return RunAsync(f => f.RunAsync(), f => f.Session.SentMessages.Should().BeEmpty());
        }
    }

    public class ProcessOutboxMessagesJobTestsFixture : FluentTestFixture
    {
        public TestableMessageSession Session { get; set; }
        public List<IClientOutboxMessageAwaitingDispatch> ClientOutboxMessages { get; set; }
        public IProcessClientOutboxMessagesJob Job { get; set; }
        public Mock<IClientOutboxStorage> ClientOutboxStorage { get; set; }

        public ProcessOutboxMessagesJobTestsFixture()
        {
            Session = new TestableMessageSession();
            ClientOutboxMessages = new List<IClientOutboxMessageAwaitingDispatch>();
            ClientOutboxStorage = new Mock<IClientOutboxStorage>();

            ClientOutboxStorage.Setup(o => o.GetAwaitingDispatchAsync()).ReturnsAsync(ClientOutboxMessages);

            Job = new ProcessClientOutboxMessagesJob(Session, ClientOutboxStorage.Object);
        }

        public Task RunAsync()
        {
            return Job.RunAsync();
        }

        public ProcessOutboxMessagesJobTestsFixture SetOutboxMessagesAwaitingDispatch()
        {
            ClientOutboxMessages.Add(new ClientOutboxMessage(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.Foo"));
            ClientOutboxMessages.Add(new ClientOutboxMessage(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.Bar"));

            return this;
        }
    }
}