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
using SFA.DAS.Validation.Exceptions;
using SFA.DAS.Validation.Mvc.Filters;
using SFA.DAS.Validation.Mvc.ModelBinding;

namespace SFA.DAS.Validation.Mvc.UnitTests.Filters;

[TestFixture]
public class ValidateModelStateFilterTests 
{
    [Test]
    public void OnActionExecuting_WhenAnActionIsExecutingAPostRequestAndTheModelStateIsValid_ThenShouldNotSetResult()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetPostRequest();
        fixture.OnActionExecuting();
        fixture.ActionExecutingContext.Result.Should().BeNull();
    }

    [Test]
    public void OnActionExecuting_WhenAnActionIsExecutingAPostRequestAndTheModelStateIsInvalid_ThenShouldSetTempDataModelState()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetPostRequest().SetInvalidModelState();
        fixture.OnActionExecuting();
        fixture.TempDataDictionary.VerifySet(t => t[typeof(SerializableModelStateDictionary).FullName] = It.IsAny<object>());
    }

    [Test]
    public void OnActionExecuting_WhenAnActionIsExecutingAPostRequestAndTheModelStateIsInvalid_ThenShouldSetRouteData()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetPostRequest().SetInvalidModelState();
        fixture.OnActionExecuting();
        fixture.RouteData.Values.Should().HaveCount(fixture.QueryString.Count);
    }

    [Test]
    public void OnActionExecuting_WhenAnActionIsExecutingAPostRequestAndTheModelStateIsInvalid_ThenShouldSetRedirectToRouteResult()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetPostRequest().SetInvalidModelState();
        fixture.OnActionExecuting();
        fixture.ActionExecutingContext.Result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>().Which.RouteValues.Should().BeEquivalentTo(fixture.RouteData.Values);
    }

    [Test]
    public void OnActionExecuting_WhenAnActionIsExecutingAGetRequestAndTheModelStateIsValid_ThenShouldNotSetResult()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetGetRequest();
        fixture.OnActionExecuting();
        fixture.ActionExecutingContext.Result.Should().BeNull();
    }

    [Test]
    public void OnActionExecuting_WhenAnActionIsExecutingAGetRequestAndTheModelStateIsInvalid_ThenShouldSetHttpBadRequestResult()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetGetRequest().SetInvalidModelState();
        fixture.OnActionExecuting();
        fixture.ActionExecutingContext.Result.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();
    }

    [Test]
    public void OnActionExecuting_WhenAnActionIsExecutingAGetRequestAndTheTempDataModelStateIsValid_ThenModelStateShouldBeValid()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetGetRequest();
        fixture.OnActionExecuting();
        fixture.ActionExecutingContext.ModelState.IsValid.Should().BeTrue();
    }

    [Test]
    public void OnActionExecuting_WhenAnActionIsExecutingAGetRequestAndTheTempDataModelStateIsInvalid_ThenModelStateShouldBeInvalid()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetGetRequest().SetInvalidTempDataModelState();
        fixture.OnActionExecuting();
        fixture.ActionExecutingContext.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void OnActionExecuted_WhenAnActionHasExecutedAPostRequestAndValidationExceptionHasNotBeenThrown_ThenShouldNotSetResult()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetPostRequest();
        fixture.OnActionExecuted();
        fixture.ActionExecutedContext.Result.Should().BeNull();
    }

    [Test]
    public void OnActionExecuted_WhenAnActionHasExecutedAPostRequestAndAValidationExceptionHasBeenThrown_ThenShouldUpdateModelState()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetPostRequest().SetValidationException();
        fixture.OnActionExecuted();
        fixture.ActionExecutedContext.ModelState.IsValid.Should().BeFalse();
    }

    [Test]
    public void OnActionExecuted_WhenAnActionHasExecutedAPostRequestAndAValidationExceptionHasBeenThrown_ThenShouldSetTempDataModelState()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetPostRequest().SetValidationException();
        fixture.OnActionExecuted();
        fixture.TempDataDictionary.VerifySet(t => t[typeof(SerializableModelStateDictionary).FullName] = It.IsAny<object>());
    }

    [Test]
    public void OnActionExecuted_WhenAnActionHasExecutedAPostRequestAndAValidationExceptionHasBeenThrown_ThenShouldSetRouteData()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetPostRequest().SetValidationException();
        fixture.OnActionExecuted();
        fixture.RouteData.Values.Should().HaveCount(fixture.QueryString.Count);
    }

    [Test]
    public void OnActionExecuted_WhenAnActionHasExecutedAPostRequestAndAValidationExceptionHasBeenThrown_ThenShouldSetRedirectToRouteResult()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetPostRequest().SetValidationException();
        fixture.OnActionExecuted();
        fixture.ActionExecutedContext.Result.Should().NotBeNull().And.BeOfType<RedirectToRouteResult>().Which.RouteValues.Should().BeEquivalentTo(fixture.RouteData.Values);
    }

    [Test]
    public void OnActionExecuted_WhenAnActionHasExecutedAPostRequestAndAValidationExceptionHasBeenThrown_ThenShouldSetExceptionHandled()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetPostRequest().SetValidationException();
        fixture.OnActionExecuted();
        fixture.ActionExecutedContext.ExceptionHandled.Should().BeTrue();
    }

    [Test]
    public void OnActionExecuted_WhenAnActionHasExecutedAGetRequestAndAValidationExceptionHasBeenThrown_ThenShouldNotSetResult()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetGetRequest().SetValidationException();
        fixture.OnActionExecuted();
        fixture.ActionExecutingContext.Result.Should().BeNull();
    }

    [Test]
    public void OnActionExecuted_WhenAnActionHasExecutedAPostRequestAndModelStateIsInvalid_ThenShouldSetTempDataModelState()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetPostRequest().SetInvalidModelState();
        fixture.OnActionExecuted();
        fixture.TempDataDictionary.VerifySet(t => t[typeof(SerializableModelStateDictionary).FullName] = It.IsAny<object>());
    }

    [Test]
    public void OnActionExecuted_WhenAnActionHasExecutedAGetRequestAndTheModelStateIsInvalid_ThenShouldNotSetTempDataModelState()
    {
        var fixture = new ValidateModelStateFilterTestsFixture();
        fixture.SetGetRequest().SetInvalidModelState();
        fixture.OnActionExecuted();
        fixture.TempDataDictionary.VerifySet(t => t[typeof(SerializableModelStateDictionary).FullName] = It.IsAny<object>(), Times.Never);
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
            new ActionContext {
                HttpContext = HttpContext.Object,
                RouteData = RouteData,
                ActionDescriptor = new ActionDescriptor(),
            },
            new List<IFilterMetadata>(),
            ActionParameters,
            Mock.Of<Controller>());

        ActionExecutedContext = new ActionExecutedContext(
            new ActionContext {
                HttpContext = HttpContext.Object,
                RouteData = RouteData,
                ActionDescriptor = new ActionDescriptor(),
            },
            new List<IFilterMetadata>(),
            Mock.Of<Controller>());

        QueryString = new QueryCollection(new Dictionary<string, StringValues> {
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
        object serializableModelStateDictionary = JsonConvert.SerializeObject(new SerializableModelStateDictionary {
            Data = new List<SerializableModelState> {
                new SerializableModelState {
                    AttemptedValue = "FooAttemptedValue",
                    ErrorMessages = new List<string> { "FooErrorMessage" },
                    Key = "Foo",
                    RawValue = "FooRawValue"
                },
                new SerializableModelState {
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