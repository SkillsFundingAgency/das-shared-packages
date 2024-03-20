using System;
using System.Web.Mvc;

using NLog;

namespace SFA.DAS.NLog.Logger.Web
{
    /// <summary>
    /// Setting new request id to the logger context
    /// Use in to GlobalFilterCollection in MVC application
    /// </summary>
    public class RequestIdActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            MappedDiagnosticsLogicalContext.Set(Constants.HeaderNameRequestCorrelationId, Guid.NewGuid());
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}