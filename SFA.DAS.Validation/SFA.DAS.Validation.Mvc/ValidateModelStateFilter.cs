#if NET462
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
                    filterContext.Result = new HttpNotFoundResult();
                }
                else
                {
                    var serializableModelState = filterContext.Controller.TempData[ModelStateKey] as SerializableModelStateDictionary;
                    var modelState = serializableModelState?.ToModelState();

                    filterContext.Controller.ViewData.ModelState.Merge(modelState);
                }
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
                filterContext.Controller.ViewData.ModelState.AddModelError(validationException);
                
                var serializableModelState = filterContext.Controller.ViewData.ModelState.ToSerializable();
                
                filterContext.Controller.TempData[ModelStateKey] = serializableModelState;
                filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.QueryString);
                filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
                filterContext.ExceptionHandled = true;
            }
            else if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                var serializableModelState = filterContext.Controller.ViewData.ModelState.ToSerializable();
                
                filterContext.Controller.TempData[ModelStateKey] = serializableModelState;
            }
        }
    }
}
#endif