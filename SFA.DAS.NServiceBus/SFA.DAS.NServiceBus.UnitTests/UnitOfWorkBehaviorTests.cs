using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests
{
    [TestFixture]
    public class UnitOfWorkBehaviorTests : FluentTest<UnitOfWorkBehaviorTestsFixture>
    {
        [Test]
        public Task Invoke_WhenHandlingAnIncomingLogicalMessage_ThenShouldSaveChangesAfterNextTask()
        {
            return RunAsync(f => f.Invoke(), f => f.Db.Verify(d => d.SaveChangesAsync(), Times.Once));
        }

        [Test]
        public Task Invoke_WhenHandlingAnIncomingLogicalMessage_ThenShouldPublishEventsAfterNextTask()
        {
            return RunAsync(f => f.Invoke(), f => f.Context.PublishedMessages.Select(m => m.Message).Should().ContainInOrder(f.Events));
        }
    }

    public class UnitOfWorkBehaviorTestsFixture : FluentTestFixture
    {
        public UnitOfWorkBehavior Behavior { get; set; }
        public TestableIncomingLogicalMessageContext Context { get; set; }
        public FakeBuilder Builder { get; set; }
        public Mock<IDb> Db { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public List<Event> Events { get; set; }
        public bool NextTaskInvoked { get; set; }

        public UnitOfWorkBehaviorTestsFixture()
        {
            Behavior = new UnitOfWorkBehavior();
            Builder = new FakeBuilder();
            Db = new Mock<IDb>();
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();

            Context = new TestableIncomingLogicalMessageContext
            {
                Builder = Builder
            };

            Events = new List<Event>
            {
                new FooEvent(),
                new BarEvent()
            };

            UnitOfWorkContext.Setup(c => c.GetEvents()).Returns(() =>
            {
                if (!NextTaskInvoked)
                    throw new Exception("GetEvents called too early");

                return Events;
            });

            Db.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask).Callback(() =>
            {
                if (!NextTaskInvoked)
                    throw new Exception("SaveChanges called too early");
            });

            Builder.Register(UnitOfWorkContext.Object);
            Builder.Register(Db.Object);
        }

        public Task Invoke()
        {
            return Behavior.Invoke(Context, () =>
            {
                NextTaskInvoked = true;

                return Task.CompletedTask;
            });
        }
    }
}