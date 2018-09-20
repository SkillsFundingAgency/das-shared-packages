#if NET462
using System.Web.Mvc;

namespace SFA.DAS.Validation.Mvc
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.HttpMethod == "GET")
            {
                var serializableModelState = filterContext.Controller.TempData.TryGet<SerializableModelStateDictionary>();
                var modelState = serializableModelState?.ToModelState();

                filterContext.Controller.ViewData.ModelState.Merge(modelState);
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
            if (filterContext.Exception is ValidationException validationException)
            {
                filterContext.Controller.ViewData.ModelState.AddModelError(filterContext.Controller.ViewData.Model, validationException);

                var serializableModelState = filterContext.Controller.ViewData.ModelState.ToSerializable();

                filterContext.Controller.TempData.Set(serializableModelState);
                filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.QueryString);

                filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
                filterContext.ExceptionHandled = true;
            }
        }
    }
}
#endif