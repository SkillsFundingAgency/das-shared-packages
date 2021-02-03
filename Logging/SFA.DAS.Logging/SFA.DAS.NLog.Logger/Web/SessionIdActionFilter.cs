using System;
using System.Web;
using System.Web.Mvc;

using NLog;

namespace SFA.DAS.NLog.Logger.Web
{
    /// <summary>
    /// Setting session id to the logger context from either the session cookie or creating a new session cookie
    /// Use in to GlobalFilterCollection in MVC application
    /// </summary>
    public class SessionIdActionFilter : IActionFilter
    {
        private readonly HttpContext _httpContext;

        public SessionIdActionFilter(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var sessionCookie = "SFA.DAS.Session";
            var cookie = _httpContext.Request.Cookies[sessionCookie];
            if (cookie == null)
            {
                var c = new HttpCookie(sessionCookie, Guid.NewGuid().ToString());
                _httpContext.Response.Cookies.Add(c);
                cookie = _httpContext.Response.Cookies[sessionCookie];
            }

            if (cookie != null)
            {
                MappedDiagnosticsLogicalContext.Set(Constants.HeaderNameSessionCorrelationId, cookie.Value);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}