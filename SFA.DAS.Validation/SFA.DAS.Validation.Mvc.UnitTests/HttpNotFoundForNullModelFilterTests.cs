#if NET462
using System.Web.Mvc;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.Validation.Mvc.UnitTests
{
    [TestFixture]
    public class HttpNotFoundForNullModelFilterTests : FluentTest<HttpNotFoundForNullModelFilterTestsFixture>
    {
        [Test]
        public void OnActionExecuted_WhenAnActionHasExecuted_ThenShouldNotSetResult()
        {
            Run(f => f.OnActionExecuted(), f => f.ActionExecutedContext.Result.Should().NotBeNull().And.BeOfType<EmptyResult>());
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAndTheModelIsNull_ThenShouldSetHttpNotFoundResult()
        {
            Run(f => f.SetNullModel(), f => f.OnActionExecuted(), f => f.ActionExecutedContext.Result.Should().NotBeNull().And.BeOfType<HttpNotFoundResult>());
        }
    }

    public class HttpNotFoundForNullModelFilterTestsFixture
    {
        public ActionExecutedContext ActionExecutedContext { get; set; }
        public HttpNotFoundForNullModelFilter HttpNotFoundForNullModelFilter { get; set; }

        public HttpNotFoundForNullModelFilterTestsFixture()
        {
            ActionExecutedContext = new ActionExecutedContext();
            HttpNotFoundForNullModelFilter = new HttpNotFoundForNullModelFilter();
        }

        public void OnActionExecuted()
        {
            HttpNotFoundForNullModelFilter.OnActionExecuted(ActionExecutedContext);
        }

        public HttpNotFoundForNullModelFilterTestsFixture SetNullModel()
        {
            ActionExecutedContext.Result = new ViewResult();

            return this;
        }
    }
}
#endif