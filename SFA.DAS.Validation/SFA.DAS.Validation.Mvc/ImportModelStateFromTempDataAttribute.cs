#if NET462
using System;
using System.Web.Mvc;

namespace SFA.DAS.Validation.Mvc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ImportModelStateFromTempDataAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var serializeableModelState = filterContext.Controller.TempData["ModelState"] as SerializableModelStateDictionary;
            var modelState = serializeableModelState?.ToModelState();

            filterContext.Controller.ViewData.ModelState.Merge(modelState);
        }
    }
}
#endif