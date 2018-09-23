#if NET462
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.Validation.Mvc
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        private const string ActionParametersKey = "__ActionParameters__";
        private const string ModelStateKey = "__ModelState__";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.ViewData.Add(ActionParametersKey, filterContext.ActionParameters);

            if (filterContext.HttpContext.Request.HttpMethod == "GET")
            {
                var serializableModelState = filterContext.Controller.TempData[ModelStateKey] as SerializableModelStateDictionary;
                var modelState = serializableModelState?.ToModelState();

                filterContext.Controller.ViewData.ModelState.Merge(modelState);
            }
            else if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                var serializableModelState = filterContext.Controller.ViewData.ModelState.ToSerializable();
                
                filterContext.Controller.TempData[ModelStateKey] = serializableModelState;
                filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.QueryString);

                filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception is ValidationException validationException)
            {
                var actionParameters = (IDictionary<string, object>)filterContext.Controller.ViewData[ActionParametersKey];
                var model = actionParameters.Values.SingleOrDefault();

                filterContext.Controller.ViewData.ModelState.AddModelError(model, validationException);

                var serializableModelState = filterContext.Controller.ViewData.ModelState.ToSerializable();
                
                filterContext.Controller.TempData[ModelStateKey] = serializableModelState;
                filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.QueryString);

                filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
                filterContext.ExceptionHandled = true;
            }
        }
    }
}
#endif