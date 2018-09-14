﻿#if NET462
using System;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.UnitOfWork.Mvc.UnitTests
{
    [TestFixture]
    public class UnitOfWorkManagerFilterTests : FluentTest<UnitOfWorkManagerFilterTestsFixture>
    {
        [Test]
        public void OnActionExecuting_WhenAnActionIsExecuting_ThenShouldBeginAUnitOfWork()
        {
            Run(f => f.OnActionExecuting(), f => f.UnitOfWorkManager.Verify(m => m.BeginAsync(), Times.Once()));
        }

        [Test]
        public void OnActionExecuting_WhenAChildActionIsExecuting_ThenShouldNotBeginAUnitOfWork()
        {
            Run(f => f.SetChildAction(), f => f.OnActionExecuting(), f => f.UnitOfWorkManager.Verify(m => m.BeginAsync(), Times.Never()));
        }

        [Test]
        public void OnActionExecuted_WhenAnResultHasExecuted_ThenShouldEndTheUnitOfWork()
        {
            Run(f => f.OnResultExecuted(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(null), Times.Once()));
        }

        [Test]
        public void OnActionExecuted_WhenAChildActionHasExecuted_ThenShouldNotEndTheUnitOfWork()
        {
            Run(f => f.SetChildAction(), f => f.OnResultExecuted(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(null), Times.Never));
        }

        [Test]
        public void OnActionExecuted_WhenAResultHasExecutedAfterAnException_ThenShouldTheEndUnitOfWork()
        {
            Run(f => f.SetException(), f => f.OnResultExecuted(), f => f.UnitOfWorkManager.Verify(m => m.EndAsync(f.Exception), Times.Once()));
        }
    }

    public class UnitOfWorkManagerFilterTestsFixture : FluentTestFixture
    {
        public Mock<IUnitOfWorkManager> UnitOfWorkManager { get; set; }
        public UnitOfWorkManagerFilter UnitOfWorkManagerFilter { get; set; }
        public RouteData RouteData { get; set; }
        public ActionExecutingContext ActionExecutingContext { get; set; }
        public ResultExecutedContext ResultExecutedContext { get; set; }
        public Exception Exception { get; set; }

        public UnitOfWorkManagerFilterTestsFixture()
        {
            UnitOfWorkManager = new Mock<IUnitOfWorkManager>();
            UnitOfWorkManagerFilter = new UnitOfWorkManagerFilter(() => UnitOfWorkManager.Object);
            RouteData = new RouteData();
            ActionExecutingContext = new ActionExecutingContext { RouteData = RouteData };
            ResultExecutedContext = new ResultExecutedContext { RouteData = RouteData };
            Exception = new Exception();
        }

        public void OnActionExecuting()
        {
            UnitOfWorkManagerFilter.OnActionExecuting(ActionExecutingContext);
        }

        public void OnResultExecuted()
        {
            UnitOfWorkManagerFilter.OnResultExecuted(ResultExecutedContext);
        }

        public UnitOfWorkManagerFilterTestsFixture SetChildAction()
        {
            RouteData.DataTokens["ParentActionViewContext"] = new object();

            return this;
        }

        public UnitOfWorkManagerFilterTestsFixture SetException()
        {
            ResultExecutedContext.Exception = Exception;

            return this;
        }
    }
}
#endif