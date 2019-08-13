#if NETCOREAPP2_0
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Validation.Mvc
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
#elif NET462
using System.Net;
using System.Web.Mvc;

namespace SFA.DAS.Validation.Mvc
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        private static readonly string ModelStateKey = typeof(SerializableModelStateDictionary).FullName;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.HttpMethod == "GET")
            {
                if (!filterContext.Controller.ViewData.ModelState.IsValid)
                {
                    filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    var serializableModelState = filterContext.Controller.TempData.Get<SerializableModelStateDictionary>();
                    var modelState = serializableModelState?.ToModelState();

                    filterContext.Controller.ViewData.ModelState.Merge(modelState);
                }
            }
            else if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                var serializableModelState = filterContext.Controller.ViewData.ModelState.ToSerializable();
                
                filterContext.Controller.TempData.Set(serializableModelState);
                filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.QueryString);
                filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Request.HttpMethod != "GET")
            {
                if (filterContext.Exception is ValidationException validationException)
                {
                    filterContext.Controller.ViewData.ModelState.AddModelError(validationException);
                    
                    var serializableModelState = filterContext.Controller.ViewData.ModelState.ToSerializable();
                    
                    filterContext.Controller.TempData.Set(serializableModelState);
                    filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.QueryString);
                    filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
                    filterContext.ExceptionHandled = true;
                }
                else if (!filterContext.Controller.ViewData.ModelState.IsValid)
                {
                    var serializableModelState = filterContext.Controller.ViewData.ModelState.ToSerializable();
                    
                    filterContext.Controller.TempData.Set(serializableModelState);
                }
            }
        }
    }
}
#endif