#if NETCOREAPP2_0
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.Validation.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpNotFoundForNullModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is ViewResult result && result.Model == null)
            {
                filterContext.Result = new NotFoundResult();
            }
        }
    }
}
#elif NET462
using System;
using System.Web.Mvc;

namespace SFA.DAS.Validation.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HttpNotFoundForNullModelAttribute : ActionFilterAttribute
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