using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing;
using SFA.DAS.UnitOfWork.EntityFramework.Pipeline;
using SFA.DAS.UnitOfWork.Pipeline;

namespace SFA.DAS.UnitOfWork.EntityFramework.UnitTests.Pipeline
{
    [TestFixture]
    public class UnitOfWorkTests : FluentTest<UnitOfWorkTestsFixture>
    {
        [Test]
        public Task CommitAsync_WhenCommittingUnitOfWork_ThenShouldSaveChangesBeforeNextTask()
        {
            return RunAsync(f => f.CommitAsync(), f => f.DbContext.Verify(d => d.SaveChangesAsync(), Times.Once()));
        }
    }

    public class UnitOfWorkTestsFixture
    {
        public Mock<DbContextStub> DbContext { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public Mock<Func<Task>> NextTask { get; set; }
        public bool NextTaskInvoked { get; set; }

        public UnitOfWorkTestsFixture()
        {
            DbContext = new Mock<DbContextStub>();
            UnitOfWork = new UnitOfWork<DbContextStub>(new Lazy<DbContextStub>(() => DbContext.Object));
            NextTask = new Mock<Func<Task>>();

            DbContext.Setup(d => d.SaveChangesAsync()).ReturnsAsync(It.IsAny<int>()).Callback(() =>
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