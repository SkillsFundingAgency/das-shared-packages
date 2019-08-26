using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using SFA.DAS.UnitOfWork.Managers;
using WebApi.StructureMap;

namespace SFA.DAS.UnitOfWork.WebApi.Filters
{
    public class UnitOfWorkManagerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            actionContext.Request.GetService<IUnitOfWorkManager>().BeginAsync().GetAwaiter().GetResult();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Request.GetService<IUnitOfWorkManager>().EndAsync(actionExecutedContext.Exception).GetAwaiter().GetResult();
        }
    }
}