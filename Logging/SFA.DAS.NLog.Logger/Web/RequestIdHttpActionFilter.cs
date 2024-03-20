using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;

using NLog;

namespace SFA.DAS.NLog.Logger.Web
{
    /// <summary>
    /// Getting request id value from header and adds it to logger context
    /// Use in HttpFilterCollection in HTTP API application
    /// </summary>
    public class RequestIdHttpActionFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var id = $"{Guid.NewGuid()}";

            IEnumerable<string> list;

            if (actionContext.Request.Headers.TryGetValues(Constants.HeaderNameRequestCorrelationId, out list))
            {
                var value = list.FirstOrDefault();
                if (value != null) id = value;
            }

            MappedDiagnosticsLogicalContext.Set(Constants.HeaderNameRequestCorrelationId, id);

            base.OnActionExecuting(actionContext);
        }
    }
}