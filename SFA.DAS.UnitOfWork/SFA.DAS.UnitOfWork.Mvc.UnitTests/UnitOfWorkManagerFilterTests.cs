#if NET462
using System;
using System.Web.Mvc;
using System.Web.Routing;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.UnitOfWork.Mvc.UnitTests
{
    [TestFixture]
    public class UnitOfWorkManagerFilterTests : FluentTest<UnitOfWorkManagerFilterTestsFixture>
    {
        [Test]
        public void OnActionExecuting_WhenAnActionIsExecuting_ThenShouldBeginUnitOfWorkManager()
        {
            Run(f => f.OnActionExecuting(), f => f.UnitOfWorkManager.Verify(m => m.BeginAsync(), Times.Once()));
        }

        [Test]
        public void OnActionExecuting_WhenAChildActionIsExecuting_ThenShouldNotBeginUnitOfWorkManager()
        {
            Run(f => f.SetChildAction(), f => f.OnActionExecuting(), f => f.UnitOfWorkManager.Verify(m => m.BeginAsync(), Times.Never()));
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecuted_ThenShouldNotEndUnitOfWorkManager()
        {
            Run(f => f.OnActionExecuted(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(null), Times.Never));
        }

        [Test]
        public void OnActionExecuted_WhenAChildActionHasExecuted_ThenShouldNotEndUnitOfWorkManager()
        {
            Run(f => f.SetChildAction(), f => f.OnActionExecuted(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(null), Times.Never));
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAfterAnUnhandledException_ThenShouldEndUnitOfWorkManager()
        {
            Run(f => f.SetActionExecutedUnhandledException(), f => f.OnActionExecuted(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(f.Exception), Times.Once));
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAfterAHandledException_ThenShouldAddExceptionToViewData()
        {
            Run(f => f.SetActionExecutedHandledException(), f => f.OnActionExecuted(), f => f.Controller.Object.ViewData["__Exception__"].Should().Be(f.Exception));
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAfterAHandledException_ThenShouldNotEndUnitOfWorkManager()
        {
            Run(f => f.SetActionExecutedHandledException(), f => f.OnActionExecuted(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(null), Times.Never));
        }

        [Test]
        public void OnResultExecuted_WhenAnResultHasExecuted_ThenShouldEndUnitOfWorkManager()
        {
            Run(f => f.OnResultExecuted(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(null), Times.Once()));
        }

        [Test]
        public void OnResultExecuted_WhenAChildActionHasExecuted_ThenShouldNotEndUnitOfWorkManager()
        {
            Run(f => f.SetChildAction(), f => f.OnResultExecuted(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(null), Times.Never));
        }

        [Test]
        public void OnResultExecuted_WhenAResultHasExecutedAfterAHandledException_ThenShouldEndUnitOfWorkManager()
        {
            Run(f => f.SetResultExecutedHandledException(), f => f.OnResultExecuted(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(f.Exception), Times.Once()));
        }

        [Test]
        public void OnResultExecuted_WhenAResultHasExecutedAfterAnException_ThenShouldEndUnitOfWorkManager()
        {
            Run(f => f.SetResultExecutedUnhandledException(), f => f.OnResultExecuted(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(f.Exception), Times.Once()));
        }
    }

    public class UnitOfWorkManagerFilterTestsFixture : FluentTestFixture
    {
        public Mock<IUnitOfWorkManager> UnitOfWorkManager { get; set; }
        public UnitOfWorkManagerFilter UnitOfWorkManagerFilter { get; set; }
        public RouteData RouteData { get; set; }
        public Mock<ControllerBase> Controller { get; set; }
        public ActionExecutingContext ActionExecutingContext { get; set; }
        public ActionExecutedContext ActionExecutedContext { get; set; }
        public ResultExecutedContext ResultExecutedContext { get; set; }
        public Exception Exception { get; set; }

        public UnitOfWorkManagerFilterTestsFixture()
        {
            UnitOfWorkManager = new Mock<IUnitOfWorkManager>();
            UnitOfWorkManagerFilter = new UnitOfWorkManagerFilter(() => UnitOfWorkManager.Object);
            RouteData = new RouteData();
            Controller = new Mock<ControllerBase>();
            ActionExecutingContext = new ActionExecutingContext { RouteData = RouteData };
            ActionExecutedContext = new ActionExecutedContext { Controller = Controller.Object, RouteData = RouteData };
            ResultExecutedContext = new ResultExecutedContext { Controller = Controller.Object, RouteData = RouteData };
            Exception = new Exception();
        }

        public void OnActionExecuted()
        {
            UnitOfWorkManagerFilter.OnActionExecuted(ActionExecutedContext);
        }

        public void OnActionExecuting()
        {
            UnitOfWorkManagerFilter.OnActionExecuting(ActionExecutingContext);
        }

        public void OnResultExecuted()
        {
            UnitOfWorkManagerFilter.OnResultExecuted(ResultExecutedContext);
        }

        public UnitOfWorkManagerFilterTestsFixture SetActionExecutedHandledException()
        {
            ActionExecutedContext.Exception = Exception;
            ActionExecutedContext.ExceptionHandled = true;

            return this;
        }

        public UnitOfWorkManagerFilterTestsFixture SetActionExecutedUnhandledException()
        {
            ActionExecutedContext.Exception = Exception;

            return this;
        }

        public UnitOfWorkManagerFilterTestsFixture SetChildAction()
        {
            RouteData.DataTokens["ParentActionViewContext"] = new object();

            return this;
        }

        public UnitOfWorkManagerFilterTestsFixture SetResultExecutedHandledException()
        {
            Controller.Object.ViewData.Add("__Exception__", Exception);

            return this;
        }

        public UnitOfWorkManagerFilterTestsFixture SetResultExecutedUnhandledException()
        {
            ResultExecutedContext.Exception = Exception;

            return this;
        }
    }
}
#endif