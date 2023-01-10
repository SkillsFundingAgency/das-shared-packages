using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Validation.Exceptions;
using SFA.DAS.Validation.Mvc.Extensions;
using SFA.DAS.Validation.Mvc.ModelBinding;

namespace SFA.DAS.Validation.Mvc.Filters
{
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
                var serializableModelState = filterContext.ModelState.ToSerializable();
                
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
}
