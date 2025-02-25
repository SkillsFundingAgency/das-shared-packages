using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Validation.Exceptions;
using SFA.DAS.Validation.Mvc.Extensions;
using SFA.DAS.Validation.Mvc.ModelBinding;

namespace SFA.DAS.Validation.Mvc.Filters;

public class ValidateModelStateFilter : ActionFilterAttribute
{
    private static readonly string ModelStateKey = typeof(SerializableModelStateDictionary).FullName;

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (filterContext.HttpContext.Request.Method == "GET")
        {
            if (!filterContext.ModelState.IsValid)
            {
                filterContext.Result = new BadRequestObjectResult(filterContext.ModelState);
            }
            else
            {
                var tempDataFactory = filterContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
                var tempData = tempDataFactory.GetTempData(filterContext.HttpContext);
                var serializableModelState = tempData.Get<SerializableModelStateDictionary>();
                var modelState = serializableModelState?.ToModelState();
                    
                filterContext.ModelState.Merge(modelState);
            }
        }
        else if (!filterContext.ModelState.IsValid)
        {
            var tempDataFactory = filterContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
            var tempData = tempDataFactory.GetTempData(filterContext.HttpContext);
                
            // Serializing the entire model state has caused issues with cookie bloat causing 400 Bad Request responses
            // when submitting a form containing a larger payload which subsequently fails validation.
            
            // By only persisting any state which is in error to the temp-data (cookies) works around this issue.
            // An example https://skillsfundingagency.atlassian.net/browse/CON-5373
            
            var serializableModelState = filterContext.ModelState.ToSerializableWithOnlyErrors();
            
            tempData.Set(serializableModelState);
            filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.Query);
            filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
        }
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        if (filterContext.HttpContext.Request.Method != "GET")
        {
            if (filterContext.Exception is ValidationException validationException)
            {
                filterContext.ModelState.AddModelError(validationException);
                    
                var tempDataFactory = filterContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
                var tempData = tempDataFactory.GetTempData(filterContext.HttpContext);
                var serializableModelState = filterContext.ModelState.ToSerializable();
                    
                tempData.Set(serializableModelState);
                filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.Query);
                filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
                filterContext.ExceptionHandled = true;
            }
            else if (!filterContext.ModelState.IsValid)
            {
                var tempDataFactory = filterContext.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
                var tempData = tempDataFactory.GetTempData(filterContext.HttpContext);
                var serializableModelState = filterContext.ModelState.ToSerializable();
                    
                tempData.Set(serializableModelState);
            }
        }
    }
}