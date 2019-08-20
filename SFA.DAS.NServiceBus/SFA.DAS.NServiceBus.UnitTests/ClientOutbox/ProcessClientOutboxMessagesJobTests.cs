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
        public Task RunAsync_WhenRunningAndClientOutboxMessagesAreAwaitingDispatch_ThenShouldSendProcessClientOutboxMessageCommands()
        {
            return RunAsync(f => f.SetClientOutboxMessagesAwaitingDispatch(), f => f.RunAsync(), f =>
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
        public Task RunAsync_WhenRunningAndNoClientOutboxMessagesAreAwaitingDispatch_ThenShouldNotSendProcessClientOutboxMessageCommands()
        {
            return RunAsync(f => f.RunAsync(), f => f.MessageSession.SentMessages.Should().BeEmpty());
        }
    }

    public class ProcessOutboxMessagesJobTestsFixture
    {
        public TestableMessageSession MessageSession { get; set; }
        public List<IClientOutboxMessageAwaitingDispatch> ClientOutboxMessages { get; set; }
        public List<IClientOutboxMessageAwaitingDispatch> ClientOutboxMessageV2s { get; set; }
        public IProcessClientOutboxMessagesJob Job { get; set; }
        public Mock<IClientOutboxStorage> ClientOutboxStorage { get; set; }
        public Mock<IClientOutboxStorageV2> ClientOutboxStorageV2 { get; set; }

        public ProcessOutboxMessagesJobTestsFixture()
        {
            MessageSession = new TestableMessageSession();
            ClientOutboxMessages = new List<IClientOutboxMessageAwaitingDispatch>();
            ClientOutboxMessageV2s = new List<IClientOutboxMessageAwaitingDispatch>();
            ClientOutboxStorage = new Mock<IClientOutboxStorage>();
            ClientOutboxStorageV2 = new Mock<IClientOutboxStorageV2>();

            ClientOutboxStorage.Setup(o => o.GetAwaitingDispatchAsync()).ReturnsAsync(ClientOutboxMessages);
            ClientOutboxStorageV2.Setup(o => o.GetAwaitingDispatchAsync()).ReturnsAsync(ClientOutboxMessageV2s);

            Job = new ProcessClientOutboxMessagesJob(MessageSession, ClientOutboxStorage.Object, ClientOutboxStorageV2.Object);
        }

        public Task RunAsync()
        {
            return Job.RunAsync();
        }

        public ProcessOutboxMessagesJobTestsFixture SetClientOutboxMessagesAwaitingDispatch()
        {
            ClientOutboxMessages.Add(new ClientOutboxMessage(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.Foo"));
            ClientOutboxMessages.Add(new ClientOutboxMessage(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.Bar"));
            ClientOutboxMessageV2s.Add(new ClientOutboxMessageV2(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.FooV2"));
            ClientOutboxMessageV2s.Add(new ClientOutboxMessageV2(GuidComb.NewGuidComb(), "SFA.DAS.NServiceBus.BarV2"));

            return this;
        }
    }
}