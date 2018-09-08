using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using SFA.DAS.NServiceBus.ClientOutbox;
using WebApi.StructureMap;

namespace SFA.DAS.NServiceBus.WebApi
{
    public class UnitOfWorkManagerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            actionContext.Request.GetService<IUnitOfWorkManager>().Begin();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Request.GetService<IUnitOfWorkManager>().End(actionExecutedContext.Exception);
        }
    }
}