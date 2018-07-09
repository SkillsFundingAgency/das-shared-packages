using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Filters;
using System.Web.Http.Hosting;
using Moq;
using NUnit.Framework;
using SFA.DAS.NServiceBus.WebApi;
using SFA.DAS.Testing;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.NServiceBus.UnitTests.WebApi
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
        public void OnActionExecuted_WhenAnActionHasExecuted_ThenShouldEndTheUnitOfWork()
        {
            Run(f => f.OnActionExecuted(), f => f.UnitOfWorkManager.Verify(m => m.End(null), Times.Once()));
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
        public HttpRequestMessage HttpRequestMessage { get; set; }
        public HttpControllerContext ControllerContext { get; set; }
        public Mock<HttpActionDescriptor> ActionDescriptor { get; set; }
        public Mock<IContainer> Container { get; set; }
        public Mock<IDependencyScope> DependencyScope { get; set; }
        public HttpActionContext ActionContext { get; set; }
        public HttpActionExecutedContext ActionExecutedContext { get; set; }
        public Exception Exception { get; set; }

        public UnitOfWorkManagerFilterTestsFixture()
        {
            UnitOfWorkManager = new Mock<IUnitOfWorkManager>();
            UnitOfWorkManagerFilter = new UnitOfWorkManagerFilter();
            HttpRequestMessage = new HttpRequestMessage();
            ControllerContext = new HttpControllerContext { Request = HttpRequestMessage };
            ActionDescriptor = new Mock<HttpActionDescriptor>();
            Container = new Mock<IContainer>();
            DependencyScope = new Mock<IDependencyScope>();
            ActionContext = new HttpActionContext(ControllerContext, ActionDescriptor.Object);
            ActionExecutedContext = new HttpActionExecutedContext(ActionContext, null) { Response = new HttpResponseMessage() };
            Exception = new Exception();

            Container.Setup(c => c.GetInstance<IUnitOfWorkManager>(It.IsAny<ExplicitArguments>())).Returns(UnitOfWorkManager.Object);
            DependencyScope.Setup(s => s.GetService(typeof(IContainer))).Returns(Container.Object);
            HttpRequestMessage.Properties[HttpPropertyKeys.DependencyScope] = DependencyScope.Object;
        }

        public void OnActionExecuting()
        {
            UnitOfWorkManagerFilter.OnActionExecuting(ActionContext);
        }

        public void OnActionExecuted()
        {
            UnitOfWorkManagerFilter.OnActionExecuted(ActionExecutedContext);
        }

        public UnitOfWorkManagerFilterTestsFixture SetException()
        {
            ActionExecutedContext.Exception = Exception;

            return this;
        }
    }
}