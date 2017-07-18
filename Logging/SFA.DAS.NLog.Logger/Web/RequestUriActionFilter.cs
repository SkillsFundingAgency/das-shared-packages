using System.Web.Mvc;

using NLog;

namespace SFA.DAS.NLog.Logger.Web
{
    public class RequestUriActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var uri = filterContext.HttpContext.Request.Url?.AbsoluteUri ?? string.Empty;
            MappedDiagnosticsLogicalContext.Set(Constants.RequestUri, uri);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}