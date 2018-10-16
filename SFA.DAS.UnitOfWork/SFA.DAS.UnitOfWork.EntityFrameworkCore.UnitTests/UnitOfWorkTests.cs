using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.UnitOfWork.EntityFrameworkCore.UnitTests
{
    [TestFixture]
    public class UnitOfWorkTests : FluentTest<UnitOfWorkTestsFixture>
    {
        [Test]
        public Task CommitAsync_WhenCommittingUnitOfWork_ThenShouldSaveChangesBeforeNextTask()
        {
            return RunAsync(f => f.CommitAsync(), f => f.DbContext.Verify(d => d.SaveChangesAsync(default(CancellationToken)), Times.Once()));
        }
    }

    public class UnitOfWorkTestsFixture
    {
        public Mock<DbContext> DbContext { get; }
        public IUnitOfWork UnitOfWork { get; set; }
        public Mock<Func<Task>> NextTask { get; set; }
        public bool NextTaskInvoked { get; set; }

        public UnitOfWorkTestsFixture()
        {
            DbContext = new Mock<DbContext>();
            UnitOfWork = new UnitOfWork<DbContext>(new Lazy<DbContext>(() => DbContext.Object));
            NextTask = new Mock<Func<Task>>();

            DbContext.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<int>()).Callback(() =>
            {
                if (NextTaskInvoked)
                    throw new Exception("SaveChanges called too late");
            });

            NextTask.Setup(n => n()).Returns(Task.CompletedTask).Callback(() => NextTaskInvoked = true);
        }

        public Task CommitAsync()
        {
            return UnitOfWork.CommitAsync(NextTask.Object);
        }
    }
}