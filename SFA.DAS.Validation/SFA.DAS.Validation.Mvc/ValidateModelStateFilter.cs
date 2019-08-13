#if NET462
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
                    var serializableModelState = filterContext.Controller.TempData[ModelStateKey] as SerializableModelStateDictionary;
                    var modelState = serializableModelState?.ToModelState();

                    filterContext.Controller.ViewData.ModelState.Merge(modelState);
                }
            }
            else if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                filterContext.Controller.TempData[ModelStateKey] = filterContext.Controller.ViewData.ModelState.ToSerializable();
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
                    filterContext.Controller.TempData[ModelStateKey] = filterContext.Controller.ViewData.ModelState.ToSerializable();
                    filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.QueryString);
                    filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
                    filterContext.ExceptionHandled = true;
                }
                else if (!filterContext.Controller.ViewData.ModelState.IsValid)
                {
                    filterContext.Controller.TempData[ModelStateKey] = filterContext.Controller.ViewData.ModelState.ToSerializable();
                }
            }
        }
    }
}
#endif