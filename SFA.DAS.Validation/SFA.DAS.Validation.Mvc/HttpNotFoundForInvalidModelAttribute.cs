#if NET462
using System;
using System.Web.Mvc;

namespace SFA.DAS.Validation.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpNotFoundForInvalidModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                filterContext.Result = new HttpNotFoundResult();
            }
        }
    }
}
#endif