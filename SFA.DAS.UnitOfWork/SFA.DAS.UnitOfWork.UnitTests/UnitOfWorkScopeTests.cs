using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing;
using StructureMap;

namespace SFA.DAS.UnitOfWork.UnitTests
{
    [TestFixture]
    public class UnitOfWorkScopeTests : FluentTest<UnitOfWorkScopeTestsFixture>
    {
        [Test]
        public Task RunAsync_WhenScopingAnOperation_ThenShouldCreateNestedContainer()
        {
            return RunAsync(f => f.RunAsync(), f => f.Container.Verify(c => c.GetNestedContainer(), Times.Once));
        }
        
        [Test]
        public Task RunAsync_WhenScopingAnOperation_ThenShouldBeginUnitOfWorkManager()
        {
            return RunAsync(f => f.RunAsync(), f => f.UnitOfWorkManager.Verify(m => m.BeginAsync(), Times.Once));
        }
        
        [Test]
        public Task RunAsync_WhenScopingAnOperation_ThenShouldInvokeOperation()
        {
            return RunAsync(f => f.RunAsync(), f => f.Operation.Verify(o => o(f.NestedContainer.Object), Times.Once));
        }
        
        [Test]
        public Task RunAsync_WhenScopingAnOperation_ThenShouldEndUnitOfWorkManager()
        {
            return RunAsync(f => f.RunAsync(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(f.Exception), Times.Once));
        }
        
        [Test]
        public Task RunAsync_WhenScopingAnOperationAndOperationThrowsException_ThenShouldEndUnitOfWorkManager()
        {
            return RunAsync(f => f.SetException(), f => f.RunAsync(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(f.Exception), Times.Once));
        }
        
        [Test]
        public Task RunAsync_WhenScopingAnOperation_ThenShouldDisposeNestedContainer()
        {
            return RunAsync(f => f.RunAsync(), f => f.NestedContainer.Verify(c => c.Dispose(), Times.Once));
        }
    }

    public class UnitOfWorkScopeTestsFixture
    {
        public Mock<IContainer> Container { get; set; }
        public Mock<Func<IContainer,Task>> Operation { get; set; }
        public IUnitOfWorkScope UnitOfWorkScope { get; set; }
        public Mock<IContainer> NestedContainer { get; set; }
        public Mock<IUnitOfWorkManager> UnitOfWorkManager { get; set; }
        public Exception Exception { get; set; }

        public UnitOfWorkScopeTestsFixture()
        {
            Container = new Mock<IContainer>();
            Operation = new Mock<Func<IContainer, Task>>();
            UnitOfWorkScope = new UnitOfWorkScope(Container.Object);
            NestedContainer = new Mock<IContainer>();
            UnitOfWorkManager = new Mock<IUnitOfWorkManager>();
            
            Container.Setup(c => c.GetNestedContainer()).Returns(NestedContainer.Object);
            NestedContainer.Setup(c => c.GetInstance<IUnitOfWorkManager>()).Returns(UnitOfWorkManager.Object);

            Operation.Setup(o => o(NestedContainer.Object)).Returns(Task.CompletedTask).Callback(() =>
            {
                NestedContainer.Verify(c => c.GetInstance<IUnitOfWorkManager>(), Times.Once);
                NestedContainer.Verify(c => c.Dispose(), Times.Never);
                UnitOfWorkManager.Verify(m => m.BeginAsync(), Times.Once);
                UnitOfWorkManager.Verify(m => m.EndAsync(It.IsAny<Exception>()), Times.Never);
            });

            UnitOfWorkManager.Setup(m => m.EndAsync(It.IsAny<Exception>())).Returns(Task.CompletedTask).Callback(() =>
            {
                NestedContainer.Verify(c => c.Dispose(), Times.Never);
            });
        }

        public Task RunAsync()
        {
            return UnitOfWorkScope.RunAsync(Operation.Object);
        }

        public UnitOfWorkScopeTestsFixture SetException()
        {
            Exception = new Exception();

            Operation.Setup(o => o(NestedContainer.Object)).ThrowsAsync(Exception);
            
            return this;
        }
    }
}