using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing;
using SFA.DAS.UnitOfWork.Managers;
using SFA.DAS.UnitOfWork.Mvc.Middleware;

namespace SFA.DAS.UnitOfWork.Mvc.UnitTests.Middleware
{
    [TestFixture]
    public class UnitOfWorkManagerMiddlewareTests : FluentTest<UnitOfWorkManagerMiddlewareTestsFixture>
    {
        [Test]
        public Task InvokeAsync_WhenInvokingMiddleware_ThenShouldBeginUnitOfWorkManagerBeforeNextTask()
        {
            return TestAsync(f => f.InvokeAsync(), f => f.UnitOfWorkManager.Verify(m => m.BeginAsync(), Times.Once));
        }

        [Test]
        public Task InvokeAsync_WhenInvokingMiddleware_ThenShouldEndUnitOfWorkManagerAfterNextTask()
        {
            return TestAsync(f => f.InvokeAsync(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(null), Times.Once));
        }

        [Test]
        public Task InvokeAsync_WhenInvokingMiddlewareAndAnExceptionIsThrown_ThenShouldEndUnitOfWorkManagerWithException()
        {
            return TestAsync(f => f.SetException(), f => f.InvokeAsyncAndSwallowException(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(f.Exception), Times.Once));
        }

        [Test]
        public Task InvokeAsync_WhenInvokingMiddlewareAndAnExceptionIsThrown_ThenShouldThrowException()
        {
            return TestExceptionAsync(f => Task.FromResult(f.SetException()), f => f.InvokeAsync(), (f, r) => Assert.ThrowsAsync<Exception>(() => r()));
        }
    }

    public class UnitOfWorkManagerMiddlewareTestsFixture
    {
        public HttpContext Context { get; set; }
        public Mock<IUnitOfWorkManager> UnitOfWorkManager { get; set; }
        public Mock<RequestDelegate> NextTask { get; set; }
        public UnitOfWorkManagerMiddleware UnitOfWorkManagerMiddleware { get; set; }
        public bool NextTaskInvoked { get; set; }
        public Exception Exception { get; set; }

        public UnitOfWorkManagerMiddlewareTestsFixture()
        {
            Context = new DefaultHttpContext();
            UnitOfWorkManager = new Mock<IUnitOfWorkManager>();
            NextTask = new Mock<RequestDelegate>();
            UnitOfWorkManagerMiddleware = new UnitOfWorkManagerMiddleware(NextTask.Object);

            UnitOfWorkManager.Setup(m => m.BeginAsync()).Returns(Task.CompletedTask).Callback(() =>
            {
                if (NextTaskInvoked)
                    throw new Exception("BeginAsync called too late");
            });

            UnitOfWorkManager.Setup(m => m.EndAsync(null)).Returns(Task.CompletedTask).Callback(() =>
            {
                if (!NextTaskInvoked)
                    throw new Exception("EndAsync called too early");
            });

            NextTask.Setup(n => n(Context)).Returns(Task.CompletedTask).Callback<HttpContext>(c => NextTaskInvoked = true);
        }

        public Task InvokeAsync()
        {
            return UnitOfWorkManagerMiddleware.InvokeAsync(Context, UnitOfWorkManager.Object);
        }

        public async Task InvokeAsyncAndSwallowException()
        {
            try
            {
                await UnitOfWorkManagerMiddleware.InvokeAsync(Context, UnitOfWorkManager.Object);
            }
            catch
            {
            }
        }

        public UnitOfWorkManagerMiddlewareTestsFixture SetException()
        {
            Exception = new Exception();

            NextTask.Setup(n => n(Context)).ThrowsAsync(Exception);

            return this;
        }
    }
}