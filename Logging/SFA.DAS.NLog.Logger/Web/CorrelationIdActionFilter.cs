using System;
using System.Web.Mvc;

using NLog;

namespace SFA.DAS.NLog.Logger.Web
{
    public class CorrelationIdActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            MappedDiagnosticsLogicalContext.Set("CorrelationId", Guid.NewGuid());
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}