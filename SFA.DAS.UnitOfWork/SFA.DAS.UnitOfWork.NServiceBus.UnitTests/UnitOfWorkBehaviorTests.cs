using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.UnitOfWork.NServiceBus.UnitTests
{
    [TestFixture]
    public class UnitOfWorkBehaviorTests : FluentTest<UnitOfWorkBehaviorTestsFixture>
    {
        [Test]
        public Task Invoke_WhenHandlingAnIncomingLogicalMessage_ThenShouldCommitUnitsOfWork()
        {
            return RunAsync(f => f.Invoke(), f => f.UnitsOfWork.ForEach(u => u.Verify(u2 => u2.CommitAsync(It.IsAny<Func<Task>>()), Times.Once())));
        }
    }

    public class UnitOfWorkBehaviorTestsFixture : FluentTestFixture
    {
        public List<Mock<IUnitOfWork>> UnitsOfWork { get; set; }
        public UnitOfWorkBehavior UnitOfWorkBehavior { get; set; }
        public FakeBuilder Builder { get; set; }
        public TestableIncomingLogicalMessageContext Context { get; set; }
        public Mock<Func<Task>> NextTask { get; set; }
        public int CommittedUnitsOfWork { get; set; }

        public UnitOfWorkBehaviorTestsFixture()
        {
            UnitsOfWork = new List<Mock<IUnitOfWork>>
            {
                new Mock<IUnitOfWork>(),
                new Mock<IUnitOfWork>(),
                new Mock<IUnitOfWork>()
            };

            UnitOfWorkBehavior = new UnitOfWorkBehavior();
            Builder = new FakeBuilder();
            Context = new TestableIncomingLogicalMessageContext { Builder = Builder };
            NextTask = new Mock<Func<Task>>();
            
            Builder.Register(UnitsOfWork.Select(u => u.Object).Concat(new[] { new NServiceBus.ClientOutbox.UnitOfWork(null, null, null, null) }).ToArray());

            NextTask.Setup(n => n()).Returns(Task.CompletedTask).Callback(() =>
            {
                if (CommittedUnitsOfWork > 0)
                    throw new Exception("Next invoked too late");
            });

            UnitsOfWork.ForEach(u => u.Setup(u2 => u2.CommitAsync(It.IsAny<Func<Task>>())).Returns<Func<Task>>(t => { CommittedUnitsOfWork++; return t(); }));
        }

        public Task Invoke()
        {
            return UnitOfWorkBehavior.Invoke(Context, NextTask.Object);
        }
    }
}