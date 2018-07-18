using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApi.StructureMap;

namespace SFA.DAS.NServiceBus.WebApi
{
    public class UnitOfWorkManagerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            actionContext.GetService<IUnitOfWorkManager>().Begin();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.GetService<IUnitOfWorkManager>().End(actionExecutedContext.Exception);
        }
    }
}