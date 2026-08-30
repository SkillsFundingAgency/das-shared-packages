using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing;
using SFA.DAS.UnitOfWork.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.Managers;

namespace SFA.DAS.UnitOfWork.UnitTests.DependencyResolution.Microsoft
{
    [TestFixture]
    public class UnitOfWorkScopeTests : FluentTest<UnitOfWorkScopeTestsFixture>
    {
        [Test]
        public Task RunAsync_WhenScopingAnOperation_ThenShouldCreateNestedServiceScope()
        {
            return TestAsync(f => f.RunAsync(), f => f.ServiceScopeFactory.Verify(s => s.CreateScope(), Times.Once));
        }
        
        [Test]
        public Task RunAsync_WhenScopingAnOperation_ThenShouldBeginUnitOfWorkManager()
        {
            return TestAsync(f => f.RunAsync(), f => f.UnitOfWorkManager.Verify(m => m.BeginAsync(), Times.Once));
        }
        
        [Test]
        public Task RunAsync_WhenScopingAnOperation_ThenShouldInvokeOperation()
        {
            return TestAsync(f => f.RunAsync(), f => f.Operation.Verify(o => o(f.NestedServiceProvider.Object), Times.Once));
        }
        
        [Test]
        public Task RunAsync_WhenScopingAnOperation_ThenShouldEndUnitOfWorkManager()
        {
            return TestAsync(f => f.RunAsync(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(f.Exception), Times.Once));
        }
        
        [Test]
        public Task RunAsync_WhenScopingAnOperationAndOperationThrowsException_ThenShouldEndUnitOfWorkManager()
        {
            return TestAsync(f => f.SetException(), f => f.RunAsyncAndSwallowException(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(f.Exception), Times.Once));
        }
        
        [Test]
        public Task RunAsync_WhenScopingAnOperationAndOperationThrowsException_ThenShouldThrowException()
        {
            return TestExceptionAsync(f => Task.FromResult(f.SetException()), f => f.RunAsync(), (f, r) => Assert.ThrowsAsync<Exception>(() => r()));
        }
        
        [Test]
        public Task RunAsync_WhenScopingAnOperation_ThenShouldDisposeNestedServiceScope()
        {
            return TestAsync(f => f.RunAsync(), f => f.NestedServiceScope.Verify(s => s.Dispose(), Times.Once));
        }
    }

    public class UnitOfWorkScopeTestsFixture
    {
        public Mock<IServiceProvider> ServiceProvider { get; set; }
        public Mock<IServiceScopeFactory> ServiceScopeFactory { get; set; }
        public Mock<Func<IServiceProvider, Task>> Operation { get; set; }
        public IUnitOfWorkScope UnitOfWorkScope { get; set; }
        public Mock<IServiceScope> NestedServiceScope { get; set; }
        public Mock<IServiceProvider> NestedServiceProvider { get; set; }
        public Mock<IUnitOfWorkManager> UnitOfWorkManager { get; set; }
        public Exception Exception { get; set; }

        public UnitOfWorkScopeTestsFixture()
        {
            ServiceProvider = new Mock<IServiceProvider>();
            ServiceScopeFactory = new Mock<IServiceScopeFactory>();
            Operation = new Mock<Func<IServiceProvider, Task>>();
            UnitOfWorkScope = new UnitOfWorkScope(ServiceProvider.Object);
            NestedServiceScope = new Mock<IServiceScope>();
            NestedServiceProvider = new Mock<IServiceProvider>();
            UnitOfWorkManager = new Mock<IUnitOfWorkManager>();
            
            ServiceProvider.Setup(p => p.GetService(typeof(IServiceScopeFactory))).Returns(ServiceScopeFactory.Object);
            ServiceScopeFactory.Setup(f => f.CreateScope()).Returns(NestedServiceScope.Object);
            NestedServiceScope.Setup(s => s.ServiceProvider).Returns(NestedServiceProvider.Object);
            NestedServiceProvider.Setup(p => p.GetService(typeof(IUnitOfWorkManager))).Returns(UnitOfWorkManager.Object);

            Operation.Setup(o => o(NestedServiceProvider.Object)).Returns(Task.CompletedTask).Callback(() =>
            {
                NestedServiceProvider.Verify(p => p.GetService(typeof(IUnitOfWorkManager)), Times.Once);
                NestedServiceScope.Verify(s => s.Dispose(), Times.Never);
                UnitOfWorkManager.Verify(m => m.BeginAsync(), Times.Once);
                UnitOfWorkManager.Verify(m => m.EndAsync(It.IsAny<Exception>()), Times.Never);
            });

            UnitOfWorkManager.Setup(m => m.EndAsync(It.IsAny<Exception>())).Returns(Task.CompletedTask).Callback(() =>
            {
                NestedServiceScope.Verify(s => s.Dispose(), Times.Never);
            });
        }

        public Task RunAsync()
        {
            return UnitOfWorkScope.RunAsync(Operation.Object);
        }
        
        public async Task RunAsyncAndSwallowException()
        {
            try
            {
                await UnitOfWorkScope.RunAsync(Operation.Object);
            }
            catch
            {
            }
        }

        public UnitOfWorkScopeTestsFixture SetException()
        {
            Exception = new Exception();

            Operation.Setup(o => o(NestedServiceProvider.Object)).ThrowsAsync(Exception);
            
            return this;
        }
    }
}