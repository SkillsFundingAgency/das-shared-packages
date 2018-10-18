using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.UnitOfWork.UnitTests
{
    [TestFixture]
    public class UnitOfWorkExtensionsTests : FluentTest<UnitOfWorkExtensionsTestsFixture>
    {
        [Test]
        public Task CommitAsync_WhenCommittingUnitsOfWork_ThenShouldCommitUnitsOfWork()
        {
            return RunAsync(f => f.UnitsOfWork.Select(u => u.Object).CommitAsync(), f => f.UnitsOfWork.ForEach(u => u.Verify(u2 => u2.CommitAsync(It.IsAny<Func<Task>>()), Times.Once)));
        }

        [Test]
        public Task CommitAsync_WhenCommittingUnitsOfWorkWithNextTask_ThenShouldInvokeNextTaskAfterCommittingUnitsOfWork()
        {
            return RunAsync(f => f.UnitsOfWork.Select(u => u.Object).CommitAsync(f.NextTask.Object), f => f.NextTask.Verify(c => c(), Times.Once()));
        }
    }

    public class UnitOfWorkExtensionsTestsFixture
    {
        public List<Mock<IUnitOfWork>> UnitsOfWork { get; }
        public Mock<Func<Task>> NextTask { get; }
        public int CommittedUnitsOfWork { get; set; }

        public UnitOfWorkExtensionsTestsFixture()
        {
            UnitsOfWork = new List<Mock<IUnitOfWork>>
            {
                new Mock<IUnitOfWork>(),
                new Mock<IUnitOfWork>(),
                new Mock<IUnitOfWork>()
            };

            NextTask = new Mock<Func<Task>>();

            NextTask.Setup(t => t()).Callback(() =>
            {
                if (CommittedUnitsOfWork != UnitsOfWork.Count)
                    throw new Exception("Invoke called too early");
            });

            UnitsOfWork.ForEach(u => u.Setup(u2 => u2.CommitAsync(It.IsAny<Func<Task>>())).Returns<Func<Task>>(t => { CommittedUnitsOfWork++; return t(); }));
        }
    }
}