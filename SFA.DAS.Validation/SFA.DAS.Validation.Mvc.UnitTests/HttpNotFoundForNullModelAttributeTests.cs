#if NET462
using System.Collections.Generic;
using System.Web.Mvc;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Testing;

namespace SFA.DAS.Validation.Mvc.UnitTests
{
    [TestFixture]
    public class HttpNotFoundForNullModelAttributeTests : FluentTest<HttpNotFoundForNullModelFilterTestsFixture>
    {
        [Test]
        public void OnActionExecuted_WhenAnActionIsExecuting_ThenShouldNotSetResult()
        {
            Run(f => f.OnActionExecuting(), f => f.ActionExecutingContext.Result.Should().BeNull());
        }

        [Test]
        public void OnActionExecuting_WhenAnActionIsExecutingAndAnActionParameterIsNull_ThenShouldSetHttpNotFoundResult()
        {
            Run(f => f.SetNullActionParameter(), f => f.OnActionExecuting(), f => f.ActionExecutingContext.Result.Should().NotBeNull().And.BeOfType<HttpNotFoundResult>());
        }
        
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
        public ActionExecutingContext ActionExecutingContext { get; set; }
        public ActionExecutedContext ActionExecutedContext { get; set; }
        public HttpNotFoundForNullModelAttribute HttpNotFoundForNullModelAttribute { get; set; }

        public HttpNotFoundForNullModelFilterTestsFixture()
        {
            ActionExecutingContext = new ActionExecutingContext { ActionParameters = new Dictionary<string, object>() };
            ActionExecutedContext = new ActionExecutedContext();
            HttpNotFoundForNullModelAttribute = new HttpNotFoundForNullModelAttribute();
        }

        public void OnActionExecuting()
        {
            HttpNotFoundForNullModelAttribute.OnActionExecuting(ActionExecutingContext);
        }

        public void OnActionExecuted()
        {
            HttpNotFoundForNullModelAttribute.OnActionExecuted(ActionExecutedContext);
        }

        public HttpNotFoundForNullModelFilterTestsFixture SetNullActionParameter()
        {
            ActionExecutingContext.ActionParameters.Add("foo", null);
            
            return this;
        }

        public HttpNotFoundForNullModelFilterTestsFixture SetNullModel()
        {
            ActionExecutedContext.Result = new ViewResult();

            return this;
        }
    }
}
#endif