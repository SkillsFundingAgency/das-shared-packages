﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NServiceBus;
using NServiceBus.UniformSession;
using NUnit.Framework;
using SFA.DAS.NServiceBus;
using SFA.DAS.Testing;

namespace SFA.DAS.UnitOfWork.NServiceBus.UnitTests
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
    }

    public class UnitOfWorkTestsFixture : FluentTestFixture
    {
        public Mock<IUniformSession> UniformSession { get; set; }
        public List<Event> Events { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public UnitOfWork UnitOfWork { get; set; }
        public Mock<Func<Task>> NextTask { get; set; }
        public bool NextTaskInvoked { get; set; }

        public UnitOfWorkTestsFixture()
        {
            UniformSession = new Mock<IUniformSession>();

            Events = new List<Event>
            {
                new FooEvent(),
                new BarEvent()
            };

            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();

            UniformSession.Setup(s => s.Publish(It.IsAny<object>(), It.IsAny<PublishOptions>())).Returns(Task.CompletedTask).Callback<object, PublishOptions>((m, o) =>
            {
                if (!NextTaskInvoked)
                    throw new Exception("Publish called too early");
            });

            UnitOfWorkContext.Setup(c => c.GetEvents()).Returns(Events);

            UnitOfWork = new UnitOfWork(UniformSession.Object, UnitOfWorkContext.Object);
            NextTask = new Mock<Func<Task>>();

            NextTask.Setup(n => n()).Returns(Task.CompletedTask).Callback(() => NextTaskInvoked = true);
        }

        public Task CommitAsync()
        {
            return UnitOfWork.CommitAsync(NextTask.Object);
        }
    }
}