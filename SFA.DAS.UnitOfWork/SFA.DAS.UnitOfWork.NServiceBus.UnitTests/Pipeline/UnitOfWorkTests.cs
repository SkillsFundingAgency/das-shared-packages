using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NServiceBus;
using NServiceBus.UniformSession;
using NUnit.Framework;
using SFA.DAS.Testing;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.UnitOfWork.NServiceBus.UnitTests.Pipeline
{
    [TestFixture]
    public class UnitOfWorkTests : FluentTest<UnitOfWorkTestsFixture>
    {
        [Test]
        public Task CommitAsync_WhenCommittingUnitOfWork_ThenShouldPublishEventsAfterNextTask()
        {
            return RunAsync(f => f.CommitAsync(), f =>
            {
                f.UniformSession.Verify(s => s.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>()), Times.Exactly(f.Events.Count));
                f.Events.ForEach(e => f.UniformSession.Verify(s => s.Publish(e, It.IsAny<PublishOptions>()), Times.Once));
            });
        }

        [Test]
        public Task CommitAsync_WhenCommittingUnitOfWork_ThenShouldPublishCommandsAfterNextTask()
        {
            return RunAsync(f => f.CommitAsync(), f =>
            {
                f.UniformSession.Verify(s => s.Send(It.IsAny<object>(), It.IsAny<SendOptions>()), Times.Exactly(f.Commands.Count));
                f.Commands.ForEach(e => f.UniformSession.Verify(s => s.Send(e, It.IsAny<SendOptions>()), Times.Once));
            });
        }
    }

    public class UnitOfWorkTestsFixture
    {
        public Mock<IUniformSession> UniformSession { get; set; }
        public List<object> Events { get; set; }
        public List<object> Commands { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public NServiceBus.Pipeline.UnitOfWork UnitOfWork { get; set; }
        public Mock<Func<Task>> NextTask { get; set; }
        public bool NextTaskInvoked { get; set; }

        public UnitOfWorkTestsFixture()
        {
            UniformSession = new Mock<IUniformSession>();

            Events = new List<object>
            {
                new FooEvent(DateTime.UtcNow),
                new BarEvent(DateTime.UtcNow)
            };

            Commands = new List<object>
            {
                new TestCommand(DateTime.UtcNow),
                new TestCommand(DateTime.UtcNow),
                new TestCommand(DateTime.UtcNow)
            };

            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();

            UniformSession.Setup(s => s.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>())).Returns(Task.CompletedTask).Callback<object, PublishOptions>((m, o) =>
            {
                if (!NextTaskInvoked)
                    throw new Exception("Publish called too early");
            });

            var allMessages = new List<object>();
            allMessages.AddRange(Events);
            allMessages.AddRange(Commands);
            UnitOfWorkContext.Setup(c => c.GetEvents()).Returns(allMessages);

            UnitOfWork = new NServiceBus.Pipeline.UnitOfWork(UniformSession.Object, UnitOfWorkContext.Object);
            NextTask = new Mock<Func<Task>>();

            NextTask.Setup(n => n()).Returns(Task.CompletedTask).Callback(() => NextTaskInvoked = true);
        }

        public Task CommitAsync()
        {
            return UnitOfWork.CommitAsync(NextTask.Object);
        }
    }
}