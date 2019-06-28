#if NET462
using System;
using System.Web.Mvc;

namespace SFA.DAS.Validation.Mvc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                filterContext.Controller.TempData["ModelState"] = filterContext.Controller.ViewData.ModelState.ToSerializable();
                filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.QueryString);
                filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception is ValidationException validationException)
            {
                filterContext.Controller.ViewData.ModelState.AddModelError(validationException);
                filterContext.Controller.TempData["ModelState"] = filterContext.Controller.ViewData.ModelState.ToSerializable();
                filterContext.RouteData.Values.Merge(filterContext.HttpContext.Request.QueryString);
                filterContext.Result = new RedirectToRouteResult(filterContext.RouteData.Values);
                filterContext.ExceptionHandled = true;
            }
        }
    }
}
#endif