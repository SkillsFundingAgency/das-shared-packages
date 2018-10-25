#if NET462
using System.Web.Mvc;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.Validation.Mvc.UnitTests
{
    [TestFixture]
    public class HttpNotFoundForInvalidModelAttributeTests : FluentTest<HttpNotFoundForInvalidModelFilterTestsFixture>
    {
        [Test]
        public void OnActionExecuted_WhenAnActionIsExecuting_ThenShouldNotSetResult()
        {
            Run(f => f.OnActionExecuted(), f => f.ActionExecutingContext.Result.Should().BeNull());
        }

        [Test]
        public void OnActionExecuted_WhenAnActionIsExecutingAndTheModelIsInvalid_ThenShouldSetHttpNotFoundResult()
        {
            Run(f => f.SetInvalidModelState(), f => f.OnActionExecuted(), f => f.ActionExecutingContext.Result.Should().NotBeNull().And.BeOfType<HttpNotFoundResult>());
        }
    }

    public class HttpNotFoundForInvalidModelFilterTestsFixture
    {
        public ActionExecutingContext ActionExecutingContext { get; set; }
        public HttpNotFoundForInvalidModelAttribute HttpNotFoundForInvalidModelAttribute { get; set; }
        public Mock<ControllerBase> Controller { get; set; }

        public HttpNotFoundForInvalidModelFilterTestsFixture()
        {
            HttpNotFoundForInvalidModelAttribute = new HttpNotFoundForInvalidModelAttribute();
            Controller = new Mock<ControllerBase>();
            ActionExecutingContext = new ActionExecutingContext { Controller = Controller.Object };
        }

        public void OnActionExecuted()
        {
            HttpNotFoundForInvalidModelAttribute.OnActionExecuting(ActionExecutingContext);
        }

        public HttpNotFoundForInvalidModelFilterTestsFixture SetInvalidModelState()
        {
            Controller.Object.ViewData.ModelState.AddModelError("Foo", "FooErrorMessage");
            Controller.Object.ViewData.ModelState.AddModelError("Bar", "BarErrorMessage");

            return this;
        }
    }
}
#endif