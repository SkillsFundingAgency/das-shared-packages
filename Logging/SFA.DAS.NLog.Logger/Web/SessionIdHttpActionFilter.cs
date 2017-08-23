using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;

using NLog;

namespace SFA.DAS.NLog.Logger.Web
{
    /// <summary>
    /// Getting session id value from header and adds it to logger context
    /// Use in HttpFilterCollection in HTTP API application
    /// </summary>
    public class SessionIdHttpActionFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var id = $"{Guid.NewGuid()}";

            IEnumerable<string> list;

            if (actionContext.Request.Headers.TryGetValues(Constants.HeaderNameSessionCorrelationId, out list))
            {
                var value = list.FirstOrDefault();
                if (value != null) id = value;
            }

            MappedDiagnosticsLogicalContext.Set(Constants.HeaderNameSessionCorrelationId, id);

            base.OnActionExecuting(actionContext);
        }
    }
}