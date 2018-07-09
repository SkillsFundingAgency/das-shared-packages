using System;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.NServiceBus.Mvc;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.Mvc
{
    [TestFixture]
    public class UnitOfWorkManagerFilterTests : FluentTest<UnitOfWorkManagerFilterTestsFixture>
    {
        [Test]
        public void OnActionExecuting_WhenAnActionIsExecuting_ThenShouldBeginAUnitOfWork()
        {
            Run(f => f.OnActionExecuting(), f => f.UnitOfWorkManager.Verify(m => m.Begin(), Times.Once()));
        }

        [Test]
        public void OnActionExecuting_WhenAChildActionIsExecuting_ThenShouldNotBeginAUnitOfWork()
        {
            Run(f => f.SetChildAction(), f => f.OnActionExecuting(), f => f.UnitOfWorkManager.Verify(m => m.Begin(), Times.Never()));
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecuted_ThenShouldEndTheUnitOfWork()
        {
            Run(f => f.OnActionExecuted(), f => f.UnitOfWorkManager.Verify(m => m.End(null), Times.Once()));
        }

        [Test]
        public void OnActionExecuted_WhenAChildActionHasExecuted_ThenShouldEndTheUnitOfWork()
        {
            Run(f => f.SetChildAction(), f => f.OnActionExecuted(), f => f.UnitOfWorkManager.Verify(m => m.End(null), Times.Never));
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAfterAnException_ThenShouldTheEndUnitOfWork()
        {
            Run(f => f.SetException(), f => f.OnActionExecuted(), f => f.UnitOfWorkManager.Verify(m => m.End(f.Exception), Times.Once()));
        }
    }

    public class UnitOfWorkManagerFilterTestsFixture : FluentTestFixture
    {
        public Mock<IUnitOfWorkManager> UnitOfWorkManager { get; set; }
        public UnitOfWorkManagerFilter UnitOfWorkManagerFilter { get; set; }
        public RouteData RouteData { get; set; }
        public ActionExecutingContext ActionExecutingContext { get; set; }
        public ActionExecutedContext ActionExecutedContext { get; set; }
        public Exception Exception { get; set; }

        public UnitOfWorkManagerFilterTestsFixture()
        {
            UnitOfWorkManager = new Mock<IUnitOfWorkManager>();
            UnitOfWorkManagerFilter = new UnitOfWorkManagerFilter(() => UnitOfWorkManager.Object);
            RouteData = new RouteData();
            ActionExecutingContext = new ActionExecutingContext { RouteData = RouteData };
            ActionExecutedContext = new ActionExecutedContext { RouteData = RouteData };
            Exception = new Exception();
        }

        public void OnActionExecuting()
        {
            UnitOfWorkManagerFilter.OnActionExecuting(ActionExecutingContext);
        }

        public void OnActionExecuted()
        {
            UnitOfWorkManagerFilter.OnActionExecuted(ActionExecutedContext);
        }

        public UnitOfWorkManagerFilterTestsFixture SetChildAction()
        {
            RouteData.DataTokens["ParentActionViewContext"] = new object();

            return this;
        }

        public UnitOfWorkManagerFilterTestsFixture SetException()
        {
            ActionExecutedContext.Exception = Exception;

            return this;
        }
    }
}