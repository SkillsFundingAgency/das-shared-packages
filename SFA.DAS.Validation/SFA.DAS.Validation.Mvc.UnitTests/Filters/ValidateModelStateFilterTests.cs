using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Testing;
using SFA.DAS.Validation.Exceptions;
using SFA.DAS.Validation.Mvc.Filters;
using SFA.DAS.Validation.Mvc.ModelBinding;

namespace SFA.DAS.Validation.Mvc.UnitTests.Filters
{
    [TestFixture]
    public class ValidateModelStateFilterTests : FluentTest<ValidateModelStateFilterTestsFixture>
    {
        [Test]
        public void OnActionExecuting_WhenAnActionIsExecutingAPostRequestAndTheModelStateIsValid_ThenShouldNotSetResult()
        {
            Run(f => f.SetPostRequest(), f => f.OnActionExecuting(), f => f.ActionExecutingContext.Result.Should().BeNull());
        }

        [Test]
        public void OnActionExecuting_WhenAnActionIsExecutingAPostRequestAndTheModelStateIsInvalid_ThenShouldSetTempDataModelState()
        {
            Run(f => f.SetPostRequest().SetInvalidModelState(), f => f.OnActionExecuting(), f => f.TempDataDictionary.VerifySet(t => t[typeof(SerializableModelStateDictionary).FullName] = It.IsAny<object>()));
        }

        [Test]
        public void OnActionExecuting_WhenAnActionIsExecutingAPostRequestAndTheModelStateIsInvalid_ThenShouldSetRouteData()
        {
            Run(f => f.SetPostRequest().SetInvalidModelState(), f => f.OnActionExecuting(), f => f.RouteData.Values.Should().HaveCount(f.QueryString.Count));
        }

        [Test]
        public void OnActionExecuting_WhenAnActionIsExecutingAPostRequestAndTheModelStateIsInvalid_ThenShouldSetRedirectToRouteResult()
        {
            Run(f => f.SetPostRequest().SetInvalidModelState(), f => f.OnActionExecuting(), f => f.ActionExecutingContext.Result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>().Which.RouteValues.Should().BeEquivalentTo(f.RouteData.Values));
        }

        [Test]
        public void OnActionExecuting_WhenAnActionIsExecutingAGetRequestAndTheModelStateIsValid_ThenShouldNotSetResult()
        {
            Run(f => f.SetGetRequest(), f => f.OnActionExecuting(), f => f.ActionExecutingContext.Result.Should().BeNull());
        }

        [Test]
        public void OnActionExecuting_WhenAnActionIsExecutingAGetRequestAndTheModelStateIsInvalid_ThenShouldSetHttpBadRequestResult()
        {
            Run(f => f.SetGetRequest().SetInvalidModelState(), f => f.OnActionExecuting(), f => f.ActionExecutingContext.Result.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>());
        }

        [Test]
        public void OnActionExecuting_WhenAnActionIsExecutingAGetRequestAndTheTempDataModelStateIsValid_ThenModelStateShouldBeValid()
        {
            Run(f => f.SetGetRequest(), f => f.OnActionExecuting(), f => f.ActionExecutingContext.ModelState.IsValid.Should().BeTrue());
        }

        [Test]
        public void OnActionExecuting_WhenAnActionIsExecutingAGetRequestAndTheTempDataModelStateIsInvalid_ThenModelStateShouldBeInvalid()
        {
            Run(f => f.SetGetRequest().SetInvalidTempDataModelState(), f => f.OnActionExecuting(), f => f.ActionExecutingContext.ModelState.IsValid.Should().BeFalse());
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAPostRequestAndValidationExceptionHasNotBeenThrown_ThenShouldNotSetResult()
        {
            Run(f => f.SetPostRequest(), f => f.OnActionExecuted(), f => f.ActionExecutedContext.Result.Should().BeNull());
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAPostRequestAndAValidationExceptionHasBeenThrown_ThenShouldUpdateModelState()
        {
            Run(f => f.SetPostRequest().SetValidationException(), f => f.OnActionExecuted(), f => f.ActionExecutedContext.ModelState.IsValid.Should().BeFalse());
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAPostRequestAndAValidationExceptionHasBeenThrown_ThenShouldSetTempDataModelState()
        {
            Run(f => f.SetPostRequest().SetValidationException(), f => f.OnActionExecuted(), f => f.TempDataDictionary.VerifySet(t => t[typeof(SerializableModelStateDictionary).FullName] = It.IsAny<object>()));
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAPostRequestAndAValidationExceptionHasBeenThrown_ThenShouldSetRouteData()
        {
            Run(f => f.SetPostRequest().SetValidationException(), f => f.OnActionExecuted(), f => f.RouteData.Values.Should().HaveCount(f.QueryString.Count));
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAPostRequestAndAValidationExceptionHasBeenThrown_ThenShouldSetRedirectToRouteResult()
        {
            Run(f => f.SetPostRequest().SetValidationException(), f => f.OnActionExecuted(), f => f.ActionExecutedContext.Result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>().Which.RouteValues.Should().BeEquivalentTo(f.RouteData.Values));
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAPostRequestAndAValidationExceptionHasBeenThrown_ThenShouldSetExceptionHandled()
        {
            Run(f => f.SetPostRequest().SetValidationException(), f => f.OnActionExecuted(), f => f.ActionExecutedContext.ExceptionHandled.Should().BeTrue());
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAGetRequestAndAValidationExceptionHasBeenThrown_ThenShouldNotSetResult()
        {
            Run(f => f.SetGetRequest().SetValidationException(), f => f.OnActionExecuted(), f => f.ActionExecutingContext.Result.Should().BeNull());
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAPostRequestAndModelStateIsInvalid_ThenShouldSetTempDataModelState()
        {
            Run(f => f.SetPostRequest().SetInvalidModelState(), f => f.OnActionExecuted(), f => f.TempDataDictionary.VerifySet(t => t[typeof(SerializableModelStateDictionary).FullName] = It.IsAny<object>()));
        }

        [Test]
        public void OnActionExecuted_WhenAnActionHasExecutedAGetRequestAndTheModelStateIsInvalid_ThenShouldNotSetTempDataModelState()
        {
            Run(f => f.SetGetRequest().SetInvalidModelState(), f => f.OnActionExecuted(), f => f.TempDataDictionary.VerifySet(t => t[typeof(SerializableModelStateDictionary).FullName] = It.IsAny<object>(), Times.Never));
        }
    }

    public class ValidateModelStateFilterTestsFixture
    {
        public ActionExecutingContext ActionExecutingContext { get; set; }
        public ActionExecutedContext ActionExecutedContext { get; set; }
        public ValidateModelStateFilter ValidateModelStateFilter { get; set; }
        public Dictionary<string, object> ActionParameters { get; set; }
        public Mock<HttpContext> HttpContext { get; set; }
        public Foo Model { get; set; }
        public RouteData RouteData { get; set; }
        public QueryCollection QueryString { get; set; }
        public Mock<IServiceProvider> ServiceProvider { get; set; }
        public Mock<ITempDataDictionaryFactory> TempDataDictionaryFactory { get; set; }
        public Mock<ITempDataDictionary> TempDataDictionary { get; set; }

        public ValidateModelStateFilterTestsFixture()
        {
            ActionParameters = new Dictionary<string, object>();
            HttpContext = new Mock<HttpContext>();
            Model = new Foo { Bar = new Bar() };
            RouteData = new RouteData();

            ActionParameters.Add("model", Model);
            
            ActionExecutingContext = new ActionExecutingContext(
                new ActionContext
                {
                    HttpContext = HttpContext.Object,
                    RouteData = RouteData,
                    ActionDescriptor = new ActionDescriptor(),
                },
                new List<IFilterMetadata>(),
                ActionParameters, 
                Mock.Of<Controller>());
            
            ActionExecutedContext = new ActionExecutedContext(
                new ActionContext
                {
                    HttpContext = HttpContext.Object,
                    RouteData = RouteData,
                    ActionDescriptor = new ActionDescriptor(),
                },
                new List<IFilterMetadata>(),
                Mock.Of<Controller>());
            
            QueryString = new QueryCollection(new Dictionary<string, StringValues>
            {
                ["Foo"] = "Foo",
                ["Bar"] = "Bar"
            });

            ServiceProvider = new Mock<IServiceProvider>();
            TempDataDictionaryFactory = new Mock<ITempDataDictionaryFactory>();
            TempDataDictionary = new Mock<ITempDataDictionary>();

            ValidateModelStateFilter = new ValidateModelStateFilter();
            
            HttpContext.Setup(c => c.Request.Query).Returns(QueryString);
            HttpContext.Setup(c => c.RequestServices).Returns(ServiceProvider.Object);
            ServiceProvider.Setup(p => p.GetService(typeof(ITempDataDictionaryFactory))).Returns(TempDataDictionaryFactory.Object);
            TempDataDictionaryFactory.Setup(f => f.GetTempData(HttpContext.Object)).Returns(TempDataDictionary.Object);
        }

        public void OnActionExecuting()
        {
            ValidateModelStateFilter.OnActionExecuting(ActionExecutingContext);
        }

        public void OnActionExecuted()
        {
            ValidateModelStateFilter.OnActionExecuted(ActionExecutedContext);
        }

        public ValidateModelStateFilterTestsFixture SetGetRequest()
        {
            HttpContext.Setup(c => c.Request.Method).Returns("GET");

            return this;
        }

        public ValidateModelStateFilterTestsFixture SetInvalidModelState()
        {
            ActionExecutingContext.ModelState.AddModelError("Foo", "FooErrorMessage");
            ActionExecutingContext.ModelState.AddModelError("Bar", "BarErrorMessage");
            ActionExecutedContext.ModelState.AddModelError("Foo", "FooErrorMessage");
            ActionExecutedContext.ModelState.AddModelError("Bar", "BarErrorMessage");

            return this;
        }

        public ValidateModelStateFilterTestsFixture SetInvalidTempDataModelState()
        {
            object serializableModelStateDictionary = JsonConvert.SerializeObject(new SerializableModelStateDictionary
            {
                Data = new List<SerializableModelState>
                {
                    new SerializableModelState
                    {
                        AttemptedValue = "FooAttemptedValue",
                        ErrorMessages = new List<string> { "FooErrorMessage" },
                        Key = "Foo",
                        RawValue = "FooRawValue"
                    },
                    new SerializableModelState
                    {
                        AttemptedValue = "BarAttemptedValue",
                        ErrorMessages = new List<string> { "BarErrorMessage" },
                        Key = "Bar",
                        RawValue = "BarRawValue"
                    }
                }
            });
            
            TempDataDictionary.Setup(t => t.TryGetValue(typeof(SerializableModelStateDictionary).FullName, out serializableModelStateDictionary)).Returns(true);

            return this;
        }

        public ValidateModelStateFilterTestsFixture SetPostRequest()
        {
            HttpContext.Setup(c => c.Request.Method).Returns("POST");

            return this;
        }

        public ValidateModelStateFilterTestsFixture SetNullActionParameter()
        {
            ActionExecutingContext.ActionArguments.Add("foo", null);
            
            return this;
        }

        public ValidateModelStateFilterTestsFixture SetValidationException()
        {
            ActionExecutedContext.Exception = new ValidationException("Oops!").AddError<Foo>(m => m.Bar.Value, "Value is invalid");

            return this;
        }
    }

    public class Foo
    {
        public Bar Bar { get; set; }
    }

    public class Bar
    {
        public int Value { get; set; }
    }
}
