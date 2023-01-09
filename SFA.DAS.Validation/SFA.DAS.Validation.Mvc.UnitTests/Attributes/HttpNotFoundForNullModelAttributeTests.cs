#if NET6_0
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing;
using SFA.DAS.Validation.Mvc.Attributes;

namespace SFA.DAS.Validation.Mvc.UnitTests.Attributes
{
    [TestFixture]
    public class HttpNotFoundForNullModelAttributeTests : FluentTest<HttpNotFoundForNullModelFilterTestsFixture>
    {
        [Test]
        public void OnActionExecuted_WhenAnActionHasExecuted_ThenShouldNotSetResult()
        {
            Run(f => f.OnActionExecuted(), f => f.ActionExecutedContext.Result.Should().BeNull());
        }
        
        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAndTheModelIsNull_ThenShouldSetHttpNotFoundResult()
        {
            Run(f => f.SetNullModel(), f => f.OnActionExecuted(), f => f.ActionExecutedContext.Result.Should().NotBeNull().And.BeOfType<NotFoundResult>());
        }
    }

    public class HttpNotFoundForNullModelFilterTestsFixture
    {
        public HttpContext HttpContext { get; set; }
        public ActionExecutedContext ActionExecutedContext { get; set; }
        public HttpNotFoundForNullModelAttribute HttpNotFoundForNullModelAttribute { get; set; }

        public HttpNotFoundForNullModelFilterTestsFixture()
        {
            HttpContext = new DefaultHttpContext();
            
            ActionExecutedContext = new ActionExecutedContext(
                new ActionContext
                {
                    HttpContext = HttpContext,
                    RouteData = new RouteData(),
                    ActionDescriptor = new ActionDescriptor(),
                },
                new List<IFilterMetadata>(),
                Mock.Of<Controller>());
            
            HttpNotFoundForNullModelAttribute = new HttpNotFoundForNullModelAttribute();
        }

        public void OnActionExecuted()
        {
            HttpNotFoundForNullModelAttribute.OnActionExecuted(ActionExecutedContext);
        }

        public HttpNotFoundForNullModelFilterTestsFixture SetNullModel()
        {
            ActionExecutedContext.Result = new ViewResult();

            return this;
        }
    }
}
#elif NET462
using System.Web.Mvc;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Testing;
using SFA.DAS.Validation.Mvc.Attributes;

namespace SFA.DAS.Validation.Mvc.UnitTests.Attributes
{
    [TestFixture]
    public class HttpNotFoundForNullModelAttributeTests : FluentTest<HttpNotFoundForNullModelFilterTestsFixture>
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
        public HttpNotFoundForNullModelAttribute HttpNotFoundForNullModelAttribute { get; set; }

        public HttpNotFoundForNullModelFilterTestsFixture()
        {
            ActionExecutedContext = new ActionExecutedContext();
            HttpNotFoundForNullModelAttribute = new HttpNotFoundForNullModelAttribute();
        }

        public void OnActionExecuted()
        {
            HttpNotFoundForNullModelAttribute.OnActionExecuted(ActionExecutedContext);
        }

        public HttpNotFoundForNullModelFilterTestsFixture SetNullModel()
        {
            ActionExecutedContext.Result = new ViewResult();

            return this;
        }
    }
}
#endif