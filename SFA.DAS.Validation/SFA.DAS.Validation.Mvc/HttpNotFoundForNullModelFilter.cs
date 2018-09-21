#if NET462
using System;
using System.Web.Mvc;

namespace SFA.DAS.Validation.Mvc
{
    public class HttpNotFoundForNullModelFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is ViewResultBase result && result.Model == null)
            {
                filterContext.Result = new HttpNotFoundResult();
            }
        }
    }
}
#endif